using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route(""), ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            CelestialObject currentCelestialObject = _context.CelestialObjects.Find(id);
            if (currentCelestialObject == null)
            {
                return NotFound();
            }
            else
            {
                currentCelestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == currentCelestialObject.Id).ToList();
                return Ok(currentCelestialObject);
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var currentCelestialObjects = _context.CelestialObjects.Where(co => co.Name == name).ToList();
            if (!currentCelestialObjects.Any())
            {
                return NotFound();
            }
            else
            {
                foreach (var currentCelestialObject in currentCelestialObjects)
                {
                    currentCelestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == currentCelestialObject.Id).ToList();
                }
                return Ok(currentCelestialObjects);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var listOfCelestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in listOfCelestialObjects)
            {
                celestialObject.Satellites = listOfCelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(listOfCelestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject cObject)
        {
            _context.CelestialObjects.Add(cObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new {  id = cObject.Id }, cObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject cObject)
        {
            CelestialObject currentCelestialObject = _context.CelestialObjects.Find(id);
            if (currentCelestialObject == null)
            {
                return NotFound();
            }
            currentCelestialObject.Name = cObject.Name;
            currentCelestialObject.OrbitalPeriod = cObject.OrbitalPeriod;
            currentCelestialObject.OrbitedObjectId = cObject.OrbitedObjectId;
            _context.Update(currentCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            CelestialObject currentCelestialObject = _context.CelestialObjects.Find(id);
            if (currentCelestialObject == null)
            {
                return NotFound();
            }
            currentCelestialObject.Name = name;
            _context.Update(currentCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjectsList = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id).ToList();
            if (!celestialObjectsList.Any())
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(celestialObjectsList);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
