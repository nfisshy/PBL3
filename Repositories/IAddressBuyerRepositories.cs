using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IAddressBuyerRepositories
    {
        AddressBuyer GetById(int id);
        void Add(AddressBuyer addressBuyer);
        void Update(AddressBuyer addressBuyer);
        void Delete(int id);
        public List<AddressBuyer> GetAllByBuyerId(int buyerId);
    }
}
