using Car_Rential.Interfaces;
using Car_Rential.Model;
using Car_Rential.Model.Validators;
using Car_Rential.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Car_Rential.Controllers
{
    [Route("api/customer")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersService _customersService;
        private readonly IValidator<InputCustomerDto> _RegisterValidator;
        private readonly IValidator<InputCustomerDto> _UpdateValidator;

        public CustomersController(
            ICustomersService customersService,
            RegisterCustomerValidator registerValidations,
            UpdateCustomerValidator updateValidations
        )
        {
            _customersService = customersService;
            _RegisterValidator = registerValidations;
            _UpdateValidator = updateValidations;
        }

        [HttpGet("all")]
        [Authorize("admin")]
        public ActionResult GetAll()
        {
            var result = _customersService.GetCustomers();

            return Ok(result);
        }

        [HttpGet]
        public ActionResult GetUser([FromQuery] int userId)
        {
            var result = _customersService.GetCustomerDto(userId);

            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult RegisterCustomer([FromBody] InputCustomerDto customerDto)
        {
            var validationResult = _RegisterValidator.Validate(customerDto);

            if (!validationResult.IsValid)
            {
                string[] errors = validationResult.FormatValidationErrors();

                return BadRequest(errors);
            }
            var result = _customersService.RegisterCustomer(customerDto);

            return Created($"/api/register/{result}", null);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult LoginCustomer([FromBody] LoginCustomerDto customerDto)
        {
            var result = _customersService.LoginCustomer(customerDto);

            return Ok(result);
        }

        [HttpDelete("{customerId}")]
        public ActionResult DeleteCustomer([FromRoute] int customerId)
        {
            _customersService.DeleteCustomer(customerId);

            return NoContent();
        }

        [HttpPatch("{customerId}")]
        public ActionResult UpdateCustomer(
            [FromBody] InputCustomerDto customerDto,
            [FromRoute] int customerId
        )
        {
            customerDto.Identyfire = customerId;
            var validationResult = _UpdateValidator.Validate(customerDto);

            if (!validationResult.IsValid)
            {
                string[] errors = validationResult.FormatValidationErrors();

                return BadRequest(errors);
            }

            _customersService.UpdateCustomer(customerDto, customerId);

            return Ok();
        }
    }
}
