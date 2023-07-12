using Car_Rential.Interfaces;
using Car_Rential.Model;
using Car_Rential.Model.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Car_Rential.Controllers
{
    [ApiController]
    [Route("api/car")]
    [Authorize("admin")]
    public class CarsController : ControllerBase
    {
        private readonly ICarsService _carsService;
        private readonly IValidator<InputCarDto> _registerCarValidator;
        private readonly IValidator<InputCarDto> _updateCarValidator;

        public CarsController(
            ICarsService carsService,
            UpdateCarValidator updateCarValidator,
            RegisterCarValidator registerCarValidator
        )
        {
            _carsService = carsService;
            _updateCarValidator = updateCarValidator;
            _registerCarValidator = registerCarValidator;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public ActionResult GetAllCars()
        {
            var result = _carsService.GetAllCars();

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCarById([FromQuery] int carId)
        {
            var result = _carsService.GetCarDto(carId);

            return Ok(result);
        }

        [HttpPost("add")]
        public ActionResult AddCar([FromBody] InputCarDto carDto)
        {
            var validationResult = _registerCarValidator.Validate(carDto);

            if (!validationResult.IsValid)
            {
                string[] errors = validationResult.FormatValidationErrors();

                return BadRequest(errors);
            }

            var result = _carsService.AddCar(carDto);
            return Created($"/api/car/{result}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCar([FromRoute] int id)
        {
            _carsService.DeleteCar(id);

            return NoContent();
        }

        [HttpPatch("{carId}")]
        public ActionResult UpdateCar([FromRoute] int carId, [FromBody] InputCarDto carDto)
        {
            carDto.Identyfire = carId;

            var validationResult = _updateCarValidator.Validate(carDto);

            if (!validationResult.IsValid)
            {
                string[] errors = validationResult.FormatValidationErrors();

                return BadRequest(errors);
            }

            _carsService.UpdateCar(carDto, carId);
            return Ok();
        }

        [HttpPatch("relocation")]
        public ActionResult RelocateCar([FromQuery] int carId, [FromQuery] int officeId)
        {
            _carsService.RelocateCar(carId, officeId);

            return Ok();
        }
    }
}
