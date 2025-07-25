using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2200004759_Triet.Data;
using _2200004759_Triet.Models;

namespace _2200004759_Triet.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly _22bitv01EmployeeContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeesController(_22bitv01EmployeeContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var _22bitv01EmployeeContext = _context.Employees.Include(e => e.Department);
            return View(await _22bitv01EmployeeContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            // Lấy danh sách phòng ban cho dropdownlist
            ViewData["Departments"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // Xử lý tệp ảnh được tải lên
                if (viewModel.PhotoImage != null)
                {
                    // 1. Validation (Kiểm tra định dạng và kích thước)
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(viewModel.PhotoImage.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("PhotoImage", "Chỉ cho phép tệp ảnh .jpg, .jpeg, .png.");
                        ViewData["Departments"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", viewModel.DepartmentId);
                        return View(viewModel);
                    }

                    if (viewModel.PhotoImage.Length > 2 * 1024 * 1024) // Giới hạn 2MB
                    {
                        ModelState.AddModelError("PhotoImage", "Kích thước tệp không được vượt quá 2MB.");
                        ViewData["Departments"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", viewModel.DepartmentId);
                        return View(viewModel);
                    }

                    // 2. Tạo đường dẫn và tên tệp duy nhất
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/photos");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.PhotoImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 3. Lưu tệp vào thư mục
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.PhotoImage.CopyToAsync(fileStream);
                    }
                }

                // 4. Chuyển dữ liệu từ ViewModel sang Model Employee
                Employee newEmployee = new Employee
                {
                    EmployeeName = viewModel.EmployeeName,
                    Gender = viewModel.Gender,
                    Email = viewModel.Email,
                    Phone = viewModel.Phone,
                    DepartmentId = viewModel.DepartmentId,
                    // 5. Lưu đường dẫn vào CSDL
                    PhotoImagePath = (uniqueFileName != null) ? "/images/photos/" + uniqueFileName : null
                };

                _context.Add(newEmployee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, tải lại dropdown list và trả về view
            ViewData["Departments"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", viewModel.DepartmentId);
            return View(viewModel);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", employee.DepartmentId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeName,Gender,Email,Phone,PhotoImagePath,DepartmentId")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
