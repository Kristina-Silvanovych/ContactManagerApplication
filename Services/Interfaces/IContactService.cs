using ContactManagerApp.Entities;

namespace ContactManagerApp.Services.Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<Contact>> GetAllContacts();
        Task<Contact> GetContactById(int id);
        Task AddContacts(IEnumerable<Contact> contacts);
        Task UpdateContact(Contact contact);
        Task DeleteContact(int id);
    }
}
