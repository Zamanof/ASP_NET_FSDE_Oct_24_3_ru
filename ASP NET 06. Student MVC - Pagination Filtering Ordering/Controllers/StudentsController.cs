using ASP_NET_06._Student_MVC___Pagination_Filtering_Ordering.Data;
using ASP_NET_06._Student_MVC___Pagination_Filtering_Ordering.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_06._Student_MVC___Pagination_Filtering_Ordering.Controllers;

public class StudentsController : Controller
{
    private readonly StudentsDbContext _context;

    public StudentsController(StudentsDbContext context)
    {
        _context = context;
    }

    // GET: Students
    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize=4,
        string nameFilter = null!,
        string lastFilter = null!,
        string orderingBy="Name",
        string orderingDirection="asc"
        )
    {
        IQueryable<Student> studentsQuery = _context.Students;

        // filter
        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            studentsQuery = studentsQuery.Where(s => s.FirstName.Contains(nameFilter));
        }

        if (!string.IsNullOrWhiteSpace(lastFilter))
        {
            studentsQuery = studentsQuery.Where(s => s.LastName.Contains(lastFilter));
        }

        // oredring
        if(orderingDirection == "asc")
        {
            studentsQuery = orderingBy switch
            {
                "Name" => studentsQuery.OrderBy(s => s.FirstName),
                "LastName" => studentsQuery.OrderBy(s => s.LastName)
            };
        }

        else if (orderingDirection == "desc")
        {
            studentsQuery = orderingBy switch
            {
                "Name" => studentsQuery.OrderByDescending(s => s.FirstName),
                "LastName" => studentsQuery.OrderByDescending(s => s.LastName)
            };
        }

        var count = await _context.Students.CountAsync();

        // Pagination
        var data = await studentsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var view = new PginationViewModel<Student>(data, page, pageSize, count);

        return View(view);
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // GET: Students/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Students/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FirstName,LastName")] Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    // POST: Students/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName")] Student student)
    {
        if (id != student.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Id))
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
        return View(student);
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }
}
