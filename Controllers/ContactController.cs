using ContactManagerApp.Entities;
using ContactManagerApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Numerics;
using System.Threading;

namespace ContactManagerApp.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var contacts = await _contactService.GetAllContacts();
            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "File is empty or not selected.");
                return View("Error"); 
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var contacts = new List<Contact>();
            int lineNumber = 1;
            var isHeader = true;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                var values = line.Split(',');

                if (values.Length != 5)
                {
                    ModelState.AddModelError("", $"Error in file format on line {lineNumber}: Incorrect number of fields.");
                    return View("Error");
                }

                try
                {
                    var name = values[0];
                    if (string.IsNullOrWhiteSpace(name))
                        throw new FormatException($"Name is empty on line {lineNumber}.");

                    var dateOfBirth = DateTime.TryParse(values[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob)
                    ? dob
                    : DateTime.MinValue;
                    var married = bool.TryParse(values[2], out var isMarried) && isMarried;
                    var phone = values[3];
                    if (string.IsNullOrWhiteSpace(phone))
                        throw new FormatException($"Phone number is empty on line {lineNumber}.");

                    var salary = decimal.TryParse(values[4], NumberStyles.Number, CultureInfo.InvariantCulture, out var sal)
                    ? sal
                    : 0;

                    contacts.Add(new Contact
                    {
                        Name = name,
                        DateOfBirth = dateOfBirth,
                        Married = married,
                        Phone = phone,
                        Salary = salary
                    });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error in file format on line {lineNumber}: {ex.Message}");
                    return View("Error");
                }

                lineNumber++;
            }

            await _contactService.AddContacts(contacts);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _contactService.DeleteContact(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("Id,Name,DateOfBirth,Married,Phone,Salary")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                var existingContact = await _contactService.GetContactById(contact.Id);
                if (existingContact == null)
                {
                    return NotFound();
                }

                existingContact.Name = contact.Name;
                existingContact.DateOfBirth = contact.DateOfBirth;
                existingContact.Married = contact.Married;
                existingContact.Phone = contact.Phone;
                existingContact.Salary = contact.Salary;

                await _contactService.UpdateContact(existingContact);

            }

            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
