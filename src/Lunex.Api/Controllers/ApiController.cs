using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api.Controllers;

[ApiController, Authorize]
public class ApiController : ControllerBase;