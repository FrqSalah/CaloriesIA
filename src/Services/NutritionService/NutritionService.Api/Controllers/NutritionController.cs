using Microsoft.AspNetCore.Mvc;
using NutritionService.Api.Models;
using NutritionService.Api.Dtos;

namespace NutritionService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NutritionController : ControllerBase
{
    private static readonly List<FoodItem> _foodItems = new()
    {
        new FoodItem { Id = Guid.NewGuid(), Name = "Apple", Calories = 52, Protein = 0.3, Carbohydrates = 14, Fat = 0.2 },
        new FoodItem { Id = Guid.NewGuid(), Name = "Banana", Calories = 89, Protein = 1.1, Carbohydrates = 23, Fat = 0.3 },
        new FoodItem { Id = Guid.NewGuid(), Name = "Chicken Breast", Calories = 165, Protein = 31, Carbohydrates = 0, Fat = 3.6 }
    };

    [HttpGet("{name}")]
    public IActionResult GetFoodItemByName(string name)
    {
        var foodItem = _foodItems.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (foodItem == null)
        {
            return NotFound();
        }

        return Ok(foodItem);
    }

    [HttpPost]
    public IActionResult AddFoodItem([FromBody] CreateFoodItemDto createDto)
    {
        if (_foodItems.Any(f => f.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict("A food item with this name already exists.");
        }

        var foodItem = new FoodItem
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Calories = createDto.Calories,
            Protein = createDto.Protein,
            Carbohydrates = createDto.Carbohydrates,
            Fat = createDto.Fat
        };

        _foodItems.Add(foodItem);

        return CreatedAtAction(
            nameof(GetFoodItemByName),
            new { name = foodItem.Name },
            foodItem
        );
    }
}
