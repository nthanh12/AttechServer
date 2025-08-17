#!/bin/bash

# CI/CD Deployment Script
# Usage: ./deploy-ci.sh [image-tag]

set -e

IMAGE_TAG=${1:-latest}
APP_NAME="attechserver"
DOMAIN="api.attech.space"

echo "🚀 Starting CI/CD deployment with tag: $IMAGE_TAG"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running"
    exit 1
fi

# Load Docker image if file exists
if [ -f "attechserver.tar" ]; then
    print_status "Loading Docker image..."
    docker load < attechserver.tar
else
    print_warning "No attechserver.tar found, building from Dockerfile..."
    docker build -t $APP_NAME:$IMAGE_TAG .
fi

# Stop existing containers
print_status "Stopping existing containers..."
docker-compose down || true
docker stop $APP_NAME || true
docker rm $APP_NAME || true

# Create uploads directory
print_status "Creating uploads directory..."
sudo mkdir -p /var/www/uploads
sudo chown -R $USER:$USER /var/www/uploads
sudo chmod -R 755 /var/www/uploads

# Update docker-compose.yml with correct image tag
print_status "Updating docker-compose.yml..."
sed -i "s/image: attechserver/image: attechserver:$IMAGE_TAG/" docker-compose.yml

# Start services with docker-compose
print_status "Starting services with docker-compose..."
docker-compose up -d

# Wait for services to start
print_status "Waiting for services to start..."
sleep 15

# Health check
print_status "Performing health check..."
if curl -f http://localhost:5000/health; then
    print_status "✅ Application is healthy!"
else
    print_error "❌ Health check failed"
    docker-compose logs
    exit 1
fi

# Setup SSL certificate if not exists
if [ ! -d "/etc/letsencrypt/live/$DOMAIN" ]; then
    print_status "Setting up SSL certificate..."
    sudo certbot --nginx -d $DOMAIN --non-interactive --agree-tos --email admin@attech.space || true
fi

# Setup auto renewal
print_status "Setting up SSL auto renewal..."
sudo crontab -l 2>/dev/null | { cat; echo "0 12 * * * /usr/bin/certbot renew --quiet"; } | sudo crontab - || true

# Final status
print_status "✅ Deployment completed successfully!"
echo ""
echo "🌐 API URL: https://$DOMAIN"
echo "📁 Uploads: https://$DOMAIN/uploads/"
echo "📊 Health: https://$DOMAIN/health"
echo ""
echo "📝 Useful commands:"
echo "  View logs: docker-compose logs -f"
echo "  Restart: docker-compose restart"
echo "  Stop: docker-compose down"
echo "  Update: ./deploy-ci.sh NEW_TAG" 