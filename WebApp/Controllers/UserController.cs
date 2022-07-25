using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValuesService _valuesService;
        private readonly IProductService _productService;

        public UserController(IUserService userService, IValuesService valuesService, IProductService productService)
        {
            _userService = userService;
            _valuesService = valuesService;
            _productService = productService;
        }

        [HttpGet(Name = "hello")]
        public string Get()
        {
            return $"{_userService.Hello()}, {_userService.Name()}, {_valuesService.Age()}, {_productService.GetProductName()}";
        }

    }
}
