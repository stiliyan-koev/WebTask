using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTask.Models;
using System.Data;
using System.Data.Entity;

namespace WebTask.Controllers
{
    public class EmployeeController : Controller
    {
        CompanyEntities db = new CompanyEntities();
        // GET: Employee
        public ActionResult Index(string option, string search, string sortBy)
        {
            
            List<Employee> employeeList = db.Employees.ToList();
            ViewBag.TotalEmployees = employeeList.Count();

            EmployeeViewModel employeeViewModel = new EmployeeViewModel();
            List<EmployeeViewModel> employeeVMList = new List<EmployeeViewModel>();
            

            if (option != null && search != null)
            {
                PerformSearch(option, search);
            }
            else
            {
                employeeVMList = employeeList.Select(x => new EmployeeViewModel
                {
                    EmployeeId = x.EmployeeId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    BirthDate = x.BirthDate,
                    Manager = x.Manager,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = db.Departments.Find(x.DepartmentId).Name,
                    Country = x.Address.Country,
                    City = x.Address.City,
                    Street = x.Address.Street

                }).ToList();

                ViewBag.FirstNameSort = String.IsNullOrEmpty(sortBy) ? "fName desc" : "";
                ViewBag.LastNameSort = sortBy == "lName" ? "lName desc" : "lName";
                ViewBag.ManagerSort = sortBy == "manager" ? "manager desc" : "manager";
                ViewBag.DepartmentSort = sortBy == "department" ? "department desc" : "department";
                switch (sortBy)
                {
                    case "fName desc":
                        employeeVMList.Sort(delegate (EmployeeViewModel e1, EmployeeViewModel e2) { return e1.FirstName.CompareTo(e2.FirstName); });
                     
                        break;
                    case "fName":
                        employeeVMList.Sort(delegate (EmployeeViewModel e1, EmployeeViewModel e2) { return e2.FirstName.CompareTo(e1.FirstName); });
                      
                        break;
                    case "lName desc":
                        employeeVMList.Sort(delegate (EmployeeViewModel e1, EmployeeViewModel e2) { return e1.LastName.CompareTo(e2.LastName); });
                        break;
                    case "lName":
                        employeeVMList.Sort(delegate (EmployeeViewModel e1, EmployeeViewModel e2) { return e2.LastName.CompareTo(e1.LastName); });
                        break;
                        
                    case "manager desc":
                        
                        employeeVMList.Sort(delegate (EmployeeViewModel e1, EmployeeViewModel e2) { return e1.Manager.CompareTo(e2.Manager); });
                        break;
                    case "manager ":
                        employeeVMList.Sort(delegate (EmployeeViewModel e1, EmployeeViewModel e2) { return e2.Manager.CompareTo(e1.Manager); });
                        break;

                    case "department desc":
                        employeeVMList.Sort(delegate (EmployeeViewModel e1, EmployeeViewModel e2) { return e1.DepartmentName.CompareTo(e2.DepartmentName); });
                        break;
                    case "department":
                        employeeVMList.Sort(delegate (EmployeeViewModel e1, EmployeeViewModel e2) { return e2.DepartmentName.CompareTo(e1.DepartmentName); });
                        break;

                }
                ViewBag.EmployeeList = employeeVMList;

            }


            return View();
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Details(int id = 0)
        {
            return View(createViewModel(db.Employees.Find(id)));
        }

        public ActionResult Edit(int id)
        {
            Employee employeeForEdit = db.Employees.Find(id);
            int selectedDepartmentId = employeeForEdit.DepartmentId;
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "Name", selectedDepartmentId);
            Employee employee = db.Employees.Find(id);
     
            return View(employee);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Employee employeeToUpdate)
        {

            if (ModelState.IsValid)
            {
                Employee updatedEmployee = db.Employees.Find(employeeToUpdate.EmployeeId);
                Address address = db.Addresses.Find(employeeToUpdate.EmployeeId);
                updatedEmployee.EmployeeId = employeeToUpdate.EmployeeId;
                updatedEmployee.FirstName = employeeToUpdate.FirstName;
                updatedEmployee.LastName = employeeToUpdate.LastName;
                updatedEmployee.BirthDate = employeeToUpdate.BirthDate;
                updatedEmployee.Manager = employeeToUpdate.Manager;
                updatedEmployee.DepartmentId = int.Parse(Request.Form["Departments"]);
                address.City = employeeToUpdate.Address.City;
                db.Addresses.Attach(address);
                updatedEmployee.Address = new Address()
                {
                    AddressId = employeeToUpdate.EmployeeId,
                    Country = employeeToUpdate.Address.Country,
                    City = employeeToUpdate.Address.City,
                    Street = employeeToUpdate.Address.Street
                };
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            return View(db.Employees.Find(id));
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult delete_conf(int id)
        {
            Employee employeeToDelete = db.Employees.Find(id);
            Address addressToDelete = db.Addresses.Find(id);
            db.Employees.Remove(employeeToDelete);
            db.Addresses.Remove(addressToDelete);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //[HttpGet]
        public ActionResult Create()
        {
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "Name", "Select department");

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeViewModel model)
        {
            Employee newEmployee = new Employee();
            newEmployee.FirstName = model.FirstName;
            newEmployee.LastName = model.LastName;
            newEmployee.BirthDate = model.BirthDate ?? DateTime.Now; ;
            newEmployee.Manager = model.Manager ?? "";
            newEmployee.DepartmentId = model.DepartmentId;
            newEmployee.Address = new Address()
            {
                Country = model.Address.Country,
                City = model.Address.City,
                Street = model.Address.Street
            };

            db.Employees.Add(newEmployee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Clean()
        {
            return RedirectToAction("Index");
        }

        public ActionResult PerformSearch(string option, string search)
        {
            List<Employee> employeeList = db.Employees.ToList();
            EmployeeViewModel employeeViewModel = new EmployeeViewModel();
            List<EmployeeViewModel> employeeVMList = new List<EmployeeViewModel>();

            if (option == "First name" && search.Length > 0)
            {
                List<Employee> employee = db.Employees.Where(x => x.FirstName == search || search == null).ToList();
                employeeVMList = getFoundDataFromSearch(employeeViewModel, employee);
                ViewBag.EmployeeList = employeeVMList;
                return View();

            }
            else if (option == "Last name" && search.Length > 0)
            {
                List<Employee> employee = db.Employees.Where(x => x.LastName == search || search == null).ToList();
                employeeVMList = getFoundDataFromSearch(employeeViewModel, employee);
                ViewBag.EmployeeList = employeeVMList;
                return View();

            }
            else if (option == "Manager" && search.Length > 0)
            {
                List<Employee> employee = db.Employees.Where(x => x.Manager == search || search == null).ToList();
                employeeVMList = getFoundDataFromSearch(employeeViewModel, employee);
                ViewBag.EmployeeList = employeeVMList;
                return View();

            }
            else
            {
                List<Employee> employee = db.Employees.ToList();
                employeeVMList = getFoundDataFromSearch(employeeViewModel, employee);
                ViewBag.EmployeeList = employeeVMList;
                return View();
            }

        }

        public List<EmployeeViewModel> getFoundDataFromSearch(EmployeeViewModel employeeViewModel, List<Employee> employee)
        {
            List<EmployeeViewModel> employeeVMList = new List<EmployeeViewModel>();
            employeeViewModel = new EmployeeViewModel();
            foreach (var x in employee)
            {
                employeeVMList.Add(new EmployeeViewModel
                {
                    EmployeeId = x.EmployeeId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    BirthDate = x.BirthDate,
                    Manager = x.Manager,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = db.Departments.Find(x.DepartmentId).Name,
                    Country = x.Address.Country,
                    City = x.Address.City,
                    Street = x.Address.Street

                });
            }
            ViewBag.TotalEmployees = employee.Count();

            return employeeVMList;
        }

        public EmployeeViewModel createViewModel(Employee employe)
        {
            EmployeeViewModel employeeViewModel = new EmployeeViewModel
            {
                EmployeeId = employe.EmployeeId,
                FirstName = employe.FirstName,
                LastName = employe.LastName,
                BirthDate = employe.BirthDate,
                Manager = employe.Manager,
                DepartmentId = employe.DepartmentId,
                DepartmentName = db.Departments.Find(employe.DepartmentId).Name,
                Country = employe.Address.Country,
                City = employe.Address.City,
                Street = employe.Address.Street

            };
            return employeeViewModel;
        }

    }



}
