using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface IParticipantRepository : IRepository<Participant>
    {
        void Update(Participant participant);
    }
}
