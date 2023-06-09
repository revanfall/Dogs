using Dogs.DataAccess.Data;
using Dogs.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Dogs.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ApiResponse _response;

        public DogsController(ApplicationDbContext context)
        {
            _context = context;
            _response = new ApiResponse();
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            _response.Result = "Dogs house service.Version 1.0.1";
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet]
        public async Task<IActionResult> GetDogs(string? attribute = null, string? order = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Dogs.AsQueryable();

            if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(order))
            {
                var property = typeof(Dog).GetProperty(attribute, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    var parameter = Expression.Parameter(typeof(Dog), "d");
                    var propertyExpression = Expression.Property(parameter, property);
                    var lambda = Expression.Lambda(propertyExpression, parameter);

                    var methodName = order.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";
                    var orderByExpression = Expression.Call(typeof(Queryable), methodName, new[] { typeof(Dog), property.PropertyType }, query.Expression, lambda);

                    query = query.Provider.CreateQuery<Dog>(orderByExpression);
                }
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            _response.Result = await query.ToListAsync();
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateDog([FromBody] Dog dog)
        {
            try
            {
                if ( dog.TailLength >= 0 && dog.Weight >= 0)
                {
                    _context.AddAsync(dog);
                    await _context.SaveChangesAsync();
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>()
                    {
                        "Can not create a dog!"
                    };
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return Ok(_response);
        }
    }
}
