using Microsoft.AspNetCore.Authorization;

namespace ASP_NET_20._TaskFlow_Files.Authorization;

public class ProjectOwnerOrAdminRequirement
    : IAuthorizationRequirement
{}
