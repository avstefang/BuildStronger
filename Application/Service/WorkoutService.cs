using Application.Dto;
using Application.Interface;
using Domain.Entity;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class WorkoutService(IWorkoutRepository workoutRepository, IEquipmentRepository equipmentRepository)
{
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;
    private readonly IEquipmentRepository _equipmentRepository = equipmentRepository;

    public async Task<IEnumerable<Workout>?> GetAllWorkoutsAsync() =>
        await _workoutRepository.GetAllWorkoutsAsync();

    public async Task<Workout?> GetWorkoutByIdAsync(Guid id) =>
        await _workoutRepository.GetWorkoutByIdAsync(id);

    public async Task CreateWorkoutAsync(CreateWorkoutDto dto)
    {
        Workout workout = new(dto.Name, dto.Description, new Duration(dto.DurationInMinutes));
        if (dto.EquipmentIds != null && dto.EquipmentIds.Count > 0)
        {
            foreach (Guid equipmentId in dto.EquipmentIds)
            {
                Equipment equipment = await _equipmentRepository.GetEquipmentByIdAsync(equipmentId)
                ?? throw new Exception("Equipment not found");
                workout.AddEquipment(equipment);
            }
        }

        await _workoutRepository.AddWorkoutAsync(workout);
    }

    public async Task UpdateWorkoutAsync(Guid id, UpdateWorkoutDto dto)
    {
        if (dto.Name == null && dto.Description == null && dto.DurationInMinutes == null)
            throw new Exception("You must provide at least one field to update");

        Workout workout = await _workoutRepository.GetWorkoutByIdAsync(id)
            ?? throw new Exception($"Workout with ID {id} not found");
        if (dto.Name != null) workout.UpdateName(dto.Name);
        if (dto.Description != null) workout.UpdateDescription(dto.Description);
        if (dto.DurationInMinutes != null) workout.UpdateDuration(new Duration(dto.DurationInMinutes ?? 0));
        await _workoutRepository.UpdateWorkoutAsync(workout);
    }

    public async Task DeleteWorkoutAsync(Guid workoutId) =>
        await _workoutRepository.DeleteWorkoutAsync(workoutId);
}