using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charity.Mvc.Models;
using Charity.Mvc.Services.Interfaces;
using Charity.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Charity.Mvc.Controllers
{
    public class DonationController : Controller
    {
        private readonly IInstitutionService _instytutionService;
        private readonly ICategoryService _categoryService;
        private readonly IDonationService _donationService;
        private readonly ICategoriesForDonations _categoriesForDonation;

        public DonationController(IInstitutionService instytutionService,
                                ICategoryService categoryService,
                                IDonationService donationService,
                                ICategoriesForDonations categoriesForDonation)
        {
            _instytutionService = instytutionService;
            _categoryService = categoryService;
            _donationService = donationService;
            _categoriesForDonation = categoriesForDonation;
        }

        
        [HttpGet]
        public IActionResult Donate()
        {
            List<string> ListaKategorii = new List<string>();

            foreach (Institution x in _instytutionService.GetAll())
            {
                ListaKategorii.Add(x.Name);
            }

            DonationViewModel donationViewModel = new DonationViewModel
            {
                DonationQuantity = 1,
                Categories = (List<Category>)_categoryService.GetAll(),
                Institutions = _instytutionService.GetAll(),
                Instytucje = new SelectList(ListaKategorii)
            };

            return View(donationViewModel);
        }

        [HttpPost]
        public IActionResult Donate(DonationViewModel donationModelView)
        {
            var newDonation = new Donation
            {
                Street = donationModelView.DonationStreet,
                Quantity = donationModelView.DonationQuantity,
                City = donationModelView.DonationCity,
                ZipCode = donationModelView.DonationZipCode,
                PickUpDate = donationModelView.DonationPickUpDate,
                PickUpTime = donationModelView.DonationPickUpTime,
                PickUpComment = donationModelView.DonationPickUpComment,
                Institutions = _instytutionService.Get(donationModelView.DonationInstitutionName)
            };

            _donationService.Create(newDonation);

            foreach (Category cat in donationModelView.Categories)
            {
                if (cat.IsChecked)
                {
                    _categoriesForDonation.Create(new CategoriesForDonations
                    {
                        CategoryId = cat.Id,
                        DonationId = newDonation.Id
                    });
                }
            }


            


            return RedirectToAction("Summary");
        }


        public IActionResult Summary()
        {
            return View();
        }
    }
        
}