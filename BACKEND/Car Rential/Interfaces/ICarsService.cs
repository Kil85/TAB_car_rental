using Car_Rential.Entieties;
using Car_Rential.Model;
using System.Linq.Expressions;

namespace Car_Rential.Interfaces
{
    public interface ICarsService
    {
        public IEnumerable<ReturnCarDto> GetAllCars();
        public int AddCar(InputCarDto carDto);
        public void DeleteCar(int carId);
        public void UpdateCar(InputCarDto carDto, int carId);
        public void RelocateCar(int carId, int officeId);
        public Car GetCarById(int carId, params Expression<Func<Car, object>>[] expressions);
        public ReturnCarDto GetCarDto(int id);
    }
}
