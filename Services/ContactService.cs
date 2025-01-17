using ContactManagerApp.Data;
using ContactManagerApp.Entities;
using ContactManagerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApp.Services
{
    public class ContactService : IContactService
    {
        private readonly ContactDBContext _context;

        public ContactService(ContactDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contact>> GetAllContacts()
        {
            return await _context.Contact.ToListAsync();
        }

        public async Task<Contact> GetContactById(int id)
        {
            return await this._context.Contact.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddContacts(IEnumerable<Contact> contacts)
        {
            await _context.Contact.AddRangeAsync(contacts);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateContact(Contact contact)
        {
            _context.Contact.Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteContact(int id)
        {
            var contact = await GetContactById(id);
            if (contact != null)
            {
                _context.Contact.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }
    }
}
