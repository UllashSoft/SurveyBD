using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyBD.Data;
using SurveyBD.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SurveyBD.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Projects.Include(p => p.Workers).ToList());
        }

        public IActionResult Details(int id)
        {
            var project = _context.Projects
                .Include(p => p.Workers)
                .Include(p => p.Data)
                .FirstOrDefault(p => p.Id == id);

            if (project == null)
                return NotFound();

            return View(project);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create() => View();

        [Authorize]
        [HttpPost]
        public IActionResult Create(string Name, List<string> workerNames, List<DateTime> Dates, List<double> RiverBedLevels, List<double> RiverBankLevels, List<double> WaterLevels)
        {
            var project = new Project
            {
                Name = Name,
                Workers = workerNames.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => new Worker { Name = n }).ToList(),
                Data = new List<RiverSurveyData>()
            };

            for (int i = 0; i < Dates.Count; i++)
            {
                project.Data.Add(new RiverSurveyData
                {
                    Date = Dates[i],
                    RiverBedLevel = RiverBedLevels[i],
                    RiverBankLevel = RiverBankLevels[i],
                    WaterLevel = WaterLevels[i]
                });
            }

            _context.Projects.Add(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var project = _context.Projects.Include(p => p.Workers).Include(p => p.Data).FirstOrDefault(p => p.Id == id);
            if (project == null)
                return NotFound();

            return View(project);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(int id, string Name, List<string> workerNames, List<DateTime> Dates, List<double> RiverBedLevels, List<double> RiverBankLevels, List<double> WaterLevels)
        {
            var project = _context.Projects.Include(p => p.Workers).Include(p => p.Data).FirstOrDefault(p => p.Id == id);
            if (project == null)
                return NotFound();

            project.Name = Name;
            project.Workers = workerNames.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => new Worker { Name = n }).ToList();
            project.Data = new List<RiverSurveyData>();

            for (int i = 0; i < Dates.Count; i++)
            {
                project.Data.Add(new RiverSurveyData
                {
                    Date = Dates[i],
                    RiverBedLevel = RiverBedLevels[i],
                    RiverBankLevel = RiverBankLevels[i],
                    WaterLevel = WaterLevels[i]
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var project = _context.Projects.Include(p => p.Workers).Include(p => p.Data).FirstOrDefault(p => p.Id == id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("No file selected");

            List<RiverSurveyData> surveyDataList = new();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        DateTime.TryParse(row.Cell(1).Value.ToString(), out DateTime date);
                        double.TryParse(row.Cell(2).Value.ToString(), out double bed);
                        double.TryParse(row.Cell(3).Value.ToString(), out double bank);
                        double.TryParse(row.Cell(4).Value.ToString(), out double water);

                        surveyDataList.Add(new RiverSurveyData
                        {
                            Date = date,
                            RiverBedLevel = bed,
                            RiverBankLevel = bank,
                            WaterLevel = water
                        });
                    }
                }
            }

            return View("ExcelGraph", surveyDataList);
        }
    }
}
