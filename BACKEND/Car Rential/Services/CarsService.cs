using AutoMapper;
using Azure.Core.Pipeline;
using Car_Rential.Entieties;
using Car_Rential.Exceptions;
using Car_Rential.Interfaces;
using Car_Rential.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace Car_Rential.Services
{
    public class CarsService : ICarsService
    {
        private readonly RentalDbContext _dbContext;
        private readonly IMapper _mapper;

        public CarsService(RentalDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<ReturnCarDto> GetAllCars()
        {
            var cars = _dbContext.Cars
                .Include(c => c.CarInfo)
                .Include(c => c.Office)
                .Include(i => i.Images)
                .ToList();

            var result = _mapper.Map<List<ReturnCarDto>>(cars);

            return result;
        }

        public ReturnCarDto GetCarDto(int id)
        {
            var car = GetCarById(id, i => i.CarInfo, o => o.Office);
            var result = _mapper.Map<ReturnCarDto>(car);
            return result;
        }

        public int AddCar(InputCarDto carDto)
        {
            var car = _mapper.Map<Car>(carDto);

            _dbContext.Cars.Add(car);
            _dbContext.SaveChanges();
            return car.Id;
        }

        public void DeleteCar(int carId)
        {
            var car = GetCarById(carId, c => c.CarInfo);

            _dbContext.Remove(car);
            _dbContext.SaveChanges();
        }

        public void UpdateCar(InputCarDto carDto, int carId)
        {
            var car = GetCarById(carId, c => c.CarInfo);

            if (carDto.Type != null)
            {
                car.Type = (string)carDto.Type;
            }
            if (carDto.Model != null)
            {
                car.Model = carDto.Model;
            }
            if (carDto.Brand != null)
            {
                car.Brand = carDto.Brand;
            }
            if (carDto.RegistrationNumber != null)
            {
                car.RegistrationNumber = carDto.RegistrationNumber;
            }
            if (carDto.pricePerDay != null)
            {
                car.pricePerDay = (int)carDto.pricePerDay;
            }
            if (carDto.SeatsNumber != null)
            {
                car.CarInfo.SeatsNumber = (int)carDto.SeatsNumber;
            }
            if (carDto.DoorsNumber != null)
            {
                car.CarInfo.DoorsNumber = (int)carDto.DoorsNumber;
            }
            if (carDto.GearboxType != null)
            {
                car.CarInfo.GearboxType = carDto.GearboxType;
            }
            if (carDto.Color != null)
            {
                car.CarInfo.Color = carDto.Color;
            }
            if (carDto.Description != null)
            {
                car.CarInfo.Description = carDto.Description;
            }
            if (carDto.FuelType != null)
            {
                car.CarInfo.FuelType = carDto.FuelType;
            }
            if (carDto.ProductionYear != null)
            {
                car.CarInfo.ProductionYear = (int)carDto.ProductionYear;
            }
            if (carDto.Mileage != null)
            {
                car.CarInfo.Mileage = (int)carDto.Mileage;
            }

            _dbContext.SaveChanges();
        }

        public void RelocateCar(int carId, int officeId)
        {
            var car = GetCarById(carId, c => c.Office);

            var office = _dbContext.Offices.FirstOrDefault(o => o.Id == officeId);
            if (office == null)
            {
                throw new CarNotFoudException("test");
            }

            car.OfficeId = officeId;
            car.Office = office;
            _dbContext.SaveChanges();
        }

        public Car GetCarById(int carId, params Expression<Func<Car, object>>[] expressions)
        {
            var car = _dbContext.Cars.Where(c => c.Id == carId);

            if (car.IsNullOrEmpty())
            {
                throw new CarNotFoudException("Car doesn't exist");
            }

            foreach (var expression in expressions)
            {
                car = car.Include(expression);
            }

            var result = car.FirstOrDefault();

            return result;
        }
    }
}
