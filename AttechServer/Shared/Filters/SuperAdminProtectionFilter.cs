using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AttechServer.Shared.Filters
{
    /// <summary>
    /// Filter bảo vệ theo Strict Hierarchy: SuperAdmin và Admin
    /// </summary>
    public class SuperAdminProtectionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContextAccessor = context.HttpContext.RequestServices
                .GetRequiredService<IHttpContextAccessor>();
            var currentUserLevel = httpContextAccessor.GetCurrentUserLevel();
            var currentUserId = httpContextAccessor.GetCurrentUserId();

            // Bảo vệ theo Strict Hierarchy
            // Kiểm tra nếu đang cố gắng thay đổi user
            var targetUserId = GetTargetUserId(context);
            if (targetUserId.HasValue)
            {
                var userService = context.HttpContext.RequestServices
                    .GetRequiredService<AttechServer.Applications.UserModules.Abstracts.IUserService>();
                
                var targetUser = userService.FindById(targetUserId.Value).Result;
                
                // SuperAdmin: chỉ SuperAdmin mới được thay đổi SuperAdmin khác
                if (targetUser.UserLevel == "system" && currentUserLevel != UserLevels.SYSTEM)
                {
                    var response = new ApiResponse(
                        ApiStatusCode.Error, 
                        null, 
                        403, 
                        "Chỉ SuperAdmin mới có quyền thay đổi thông tin SuperAdmin"
                    );
                    
                    context.Result = new JsonResult(response) { StatusCode = 403 };
                    return;
                }

                // Admin: không được thay đổi Admin khác (chỉ quản lý STAFF)
                if (targetUser.UserLevel == "manager" && currentUserLevel == UserLevels.MANAGER)
                {
                    var response = new ApiResponse(
                        ApiStatusCode.Error,
                        null,
                        403,
                        "Admin chỉ được quản lý STAFF, không thể thay đổi Admin khác"
                    );
                    
                    context.Result = new JsonResult(response) { StatusCode = 403 };
                    return;
                }
            }

            // Kiểm tra nếu đang cố gắng tạo user
            var createUserDto = context.ActionArguments.Values
                .FirstOrDefault(arg => arg?.GetType().Name.Contains("CreateUserDto") == true);
            if (createUserDto != null)
            {
                var userLevelProperty = createUserDto.GetType().GetProperty("UserLevel");
                if (userLevelProperty != null)
                {
                    var userLevel = (int)userLevelProperty.GetValue(createUserDto);
                    
                    // Chỉ SuperAdmin mới có thể tạo SuperAdmin khác
                    if (userLevel == UserLevels.SYSTEM && currentUserLevel != UserLevels.SYSTEM)
                    {
                        var response = new ApiResponse(
                            ApiStatusCode.Error,
                            null,
                            403,
                            "Chỉ SuperAdmin mới có thể tạo SuperAdmin khác"
                        );
                        
                        context.Result = new JsonResult(response) { StatusCode = 403 };
                        return;
                    }

                    // Chỉ SuperAdmin mới có thể tạo Admin (Admin không thể tạo Admin khác)
                    if (userLevel == UserLevels.MANAGER && currentUserLevel != UserLevels.SYSTEM)
                    {
                        var response = new ApiResponse(
                            ApiStatusCode.Error,
                            null,
                            403,
                            "Chỉ SuperAdmin mới có thể tạo Admin khác"
                        );
                        
                        context.Result = new JsonResult(response) { StatusCode = 403 };
                        return;
                    }
                }
            }

            // Kiểm tra nếu đang cố gắng update user level
            var updateUserDto = context.ActionArguments.Values
                .FirstOrDefault(arg => arg?.GetType().Name.Contains("UpdateUserDto") == true);
            if (updateUserDto != null)
            {
                var userLevelProperty = updateUserDto.GetType().GetProperty("UserLevel");
                if (userLevelProperty != null)
                {
                    var userLevel = (int)userLevelProperty.GetValue(updateUserDto);
                    
                    // Chỉ SuperAdmin mới có thể nâng cấp user lên SuperAdmin
                    if (userLevel == UserLevels.SYSTEM && currentUserLevel != UserLevels.SYSTEM)
                    {
                        var response = new ApiResponse(
                            ApiStatusCode.Error,
                            null,
                            403,
                            "Chỉ SuperAdmin mới có thể nâng cấp user lên SuperAdmin"
                        );
                        
                        context.Result = new JsonResult(response) { StatusCode = 403 };
                        return;
                    }

                    // Chỉ SuperAdmin mới có thể nâng cấp user lên Admin
                    if (userLevel == UserLevels.MANAGER && currentUserLevel != UserLevels.SYSTEM)
                    {
                        var response = new ApiResponse(
                            ApiStatusCode.Error,
                            null,
                            403,
                            "Chỉ SuperAdmin mới có thể nâng cấp user lên Admin"
                        );
                        
                        context.Result = new JsonResult(response) { StatusCode = 403 };
                        return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }

        private int? GetTargetUserId(ActionExecutingContext context)
        {
            // Lấy userId từ route parameter
            if (context.RouteData.Values.TryGetValue("id", out var idValue))
            {
                if (int.TryParse(idValue?.ToString(), out var userId))
                {
                    return userId;
                }
            }

            // Lấy userId từ DTO
            var dto = context.ActionArguments.Values
                .FirstOrDefault(arg => arg?.GetType().GetProperty("Id") != null);
            if (dto != null)
            {
                var idProperty = dto.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var idVal = idProperty.GetValue(dto);
                    if (idVal is int id && id > 0)
                    {
                        return id;
                    }
                }
            }

            return null;
        }
    }
}
