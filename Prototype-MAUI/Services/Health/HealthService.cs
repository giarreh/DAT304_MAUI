﻿using MauiApp8.Services.BackgroundServices.Realm;

namespace MauiApp8.Services.Health
{
    internal class HealthService : IHealthService
    {
        private readonly IUtils _utils;
        private readonly ICRUD _crud;

        public HealthService(IUtils utils, ICRUD crud)
        {
            _utils = utils;
            _crud = crud;
        }

        public List<Model.GlucoseInfo> ReadGlucoses(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadGlucoses(realm, fromDate, toDate);
        }

        public List<Model.InsulinInfo> ReadInsulins(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadInsulins(realm, fromDate, toDate);
        }

        public List<Model.Food> ReadFoods()
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadFoods(realm);
        }

        public Model.Food ReadFood(string name)
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadFood(realm, name);
        }

        public async Task AddFood(string name, float calories, float carbohydrates, float protein, float fat)
        {
            using var realm = _utils.RealmCreate();
            await _crud.AddFood(realm, name, calories, carbohydrates, protein, fat);
        }

        public async Task UpdateFood(string name, float calories, float carbohydrates, float protein, float fat)
        {
            using var realm = _utils.RealmCreate();
            await _crud.UpdateFood(realm, name, calories, carbohydrates, protein, fat);
        }

        public async Task DeleteFood(string foodName)
        {
            using var realm = _utils.RealmCreate();
            await _crud.DeleteFood(realm, foodName);
        }

        public List<Model.FoodEntry> ReadFoodEntries()
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadFoodEntries(realm);
        }

        public Model.FoodEntry ReadFoodEntry(int foodEntryId)
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadFoodEntry(realm, foodEntryId);
        }

        public async Task<int> CreateFoodEntry(string foodName, float amount)
        {
            using var realm = _utils.RealmCreate();
            return await _crud.CreateFoodEntry(realm, foodName, amount);
        }

        public async Task DeleteFoodEntry(int foodEntryId)
        {
            using var realm = _utils.RealmCreate();
            await _crud.DeleteFoodEntry(realm, foodEntryId);
        }

        public List<Model.Meal> ReadMeals()
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadMeals(realm);
        }

        public Model.Meal ReadMeal(int mealId)
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadMeal(realm, mealId);
        }

        public async Task<int> CreateMeal(List<int> foodEntryIds)
        {
            using var realm = _utils.RealmCreate();
            return await _crud.CreateMeal(realm, foodEntryIds);
        }

        public async Task DeleteMeal(int mealId)
        {
            using var realm = _utils.RealmCreate();
            await _crud.DeleteMeal(realm, mealId);
        }
    }
}