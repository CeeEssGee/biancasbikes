using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BiancasBikes.Data;
using Microsoft.EntityFrameworkCore;
using BiancasBikes.Models;

namespace BiancasBikes.Controllers;

[ApiController] // all controllers in program should be decorated with this
[Route("api/[controller]")] // The BikeController class also has a Route attribute. This tells the framework what route segment should be associated with all of the endpoints in the controller. In this case, "api/[controller]" tells the framework to use the first part of the controller name ("Bike") to create the route. So all of the endpoints in this controller will have URLs that start with "/api/bike" (it is case insensitive)
public class BikeController : ControllerBase // all controllers in program should inherit ControllerBase
{
    private BiancasBikesDbContext _dbContext;

    public BikeController(BiancasBikesDbContext context)
    {
        _dbContext = context;
    }

    // A controller contains all of the endpoints for a specific resource. In this case, all endpoints related to the Bikes resource. Usually you will have one controller for each resource available in the API.

    [HttpGet] // It is decorated with the HttpGet attribute to mark it as a GET endpoint, but is technically unnecessary as GET is the default.
    [Authorize] // Authorize will be covered in the next chapter.
    public IActionResult Get() // Finally, the Get method is an endpoint.

    {
        return Ok(_dbContext.Bikes.Include(b => b.Owner).ToList()); // The Ok method that gets called inside Get will create an HTTP response with a status of 200, as well as the data that's passed in.
    }

    [HttpGet("{id}")] // "{id}" has been passed into HttpGet to add more to the route that this handler should map to. Because the controller's route is already /api/bike, adding {id} will make the whole route for this endpoint be /api/bike/{id}. Just like when we were using Minimal APIs, the name inside the {} must match the int id param in the method in order for the framework to pass the value in from the URL when calling it.
    [Authorize]
    public IActionResult GetById(int id)
    {
        Bike bike = _dbContext
            .Bikes
            .Include(b => b.Owner)
            .Include(b => b.BikeType)
            .Include(b => b.WorkOrders)
            .SingleOrDefault(b => b.Id == id);

        if (bike == null)
        {
            return NotFound();
        }

        return Ok(bike);
    }

    // get Garage Inventory / incomplete work orders
    [HttpGet("inventory")] // route is /api/bike/inventory
    [Authorize]
    public IActionResult Inventory() // Knows it's a get as per the HttpGet above
    {
        int inventory = _dbContext
        .Bikes
        .Where(b => b.WorkOrders.Any(wo => wo.DateCompleted == null))
        .Count();

        return Ok(inventory);
    }

}