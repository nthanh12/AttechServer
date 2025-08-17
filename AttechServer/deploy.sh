#!/bin/bash

# Production Deployment Script for AttechServer
# Usage: ./deploy.sh

set -e

echo "🚀 Starting deployment..."

# Variables
APP_NAME="attechserver"
DOMAIN="api.attech.space"
UPLOADS_DIR="/var/www/uploads"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running as root
if [[ $EUID -eq 0 ]]; then
   print_error "This script should not be run as root"
   exit 1
fi

# Update system packages
print_status "Updating system packages..."
sudo apt update && sudo apt upgrade -y

# Install required packages
print_status "Installing required packages..."
sudo apt install -y nginx certbot python3-certbot-nginx docker.io docker-compose curl

# Start and enable Docker
print_status "Starting Docker service..."
sudo systemctl start docker
sudo systemctl enable docker

# Add user to docker group
sudo usermod -aG docker $USER

# Create uploads directory
print_status "Creating uploads directory..."
sudo mkdir -p $UPLOADS_DIR
sudo chown -R $USER:$USER $UPLOADS_DIR
sudo chmod -R 755 $UPLOADS_DIR

# Build and run Docker container
print_status "Building Docker image..."
docker build -t $APP_NAME .

# Stop existing container if running
print_status "Stopping existing container..."
docker stop $APP_NAME || true
docker rm $APP_NAME || true

# Run new container
print_status "Starting new container..."
docker run -d \
    --name $APP_NAME \
    --restart unless-stopped \
    -p 5000:5000 \
    -v $UPLOADS_DIR:/app/uploads \
    -e ASPNETCORE_ENVIRONMENT=Production \
    -e BASE_URL=https://$DOMAIN \
    $APP_NAME

# Wait for container to start
print_status "Waiting for container to start..."
sleep 10

# Check if container is running
if docker ps | grep -q $APP_NAME; then
    print_status "Container is running successfully"
else
    print_error "Container failed to start"
    docker logs $APP_NAME
    exit 1
fi

# Configure nginx
print_status "Configuring nginx..."
sudo cp nginx.conf /etc/nginx/sites-available/$DOMAIN
sudo ln -sf /etc/nginx/sites-available/$DOMAIN /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/default

# Test nginx configuration
if sudo nginx -t; then
    print_status "Nginx configuration is valid"
else
    print_error "Nginx configuration is invalid"
    exit 1
fi

# Reload nginx
sudo systemctl reload nginx

# Setup SSL certificate
print_status "Setting up SSL certificate..."
sudo certbot --nginx -d $DOMAIN --non-interactive --agree-tos --email admin@attech.space

# Setup automatic renewal
print_status "Setting up automatic SSL renewal..."
sudo crontab -l 2>/dev/null | { cat; echo "0 12 * * * /usr/bin/certbot renew --quiet"; } | sudo crontab -

# Final health check
print_status "Performing health check..."
sleep 5
if curl -f https://$DOMAIN/health; then
    print_status "✅ Deployment completed successfully!"
    echo ""
    echo "🌐 API URL: https://$DOMAIN"
    echo "📁 Uploads: https://$DOMAIN/uploads/"
    echo "📊 Health: https://$DOMAIN/health"
    echo ""
    echo "📝 Useful commands:"
    echo "  View logs: docker logs $APP_NAME"
    echo "  Restart: docker restart $APP_NAME"
    echo "  Update: ./deploy.sh"
else
    print_error "❌ Health check failed"
    exit 1
fi 