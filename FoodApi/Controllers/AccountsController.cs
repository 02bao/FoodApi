using AuthenticationPlugin;
using FoodApi.Data;
using Microsoft.AspNetCore.Mvc;


namespace FoodApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountsController : ControllerBase
    {
        private  FoodDbContext _dbContext;
        private  IConfiguration _configuration;
        private readonly AuthService _auth;

        public AccountsController(FoodDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }
    }
}
