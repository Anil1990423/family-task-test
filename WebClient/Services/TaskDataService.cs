using Domain.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using WebClient.Abstractions;
using WebClient.Shared.Models;
using Domain.Queries;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Domain.ViewModel;
using Core.Extensions.ModelConversion;


namespace WebClient.Services
{
    public class TaskDataService : ITaskDataService
    {
        private readonly HttpClient httpClient;
        public TaskDataService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("FamilyTaskAPI");
            Tasks = new List<TaskVm>();
            LoadTasks();
            //Tasks = new List<TaskModel>();
        }
        //public List<TaskModel> Tasks { get; private set; }
        public List<TaskVm> Tasks { get; private set; }
        public TaskVm SelectedTask { get; private set; }
        public bool showAllTasks { get; private set; }


        public event EventHandler TasksUpdated;
        public event EventHandler TaskSelected;

        public void SelectTask(Guid id)
        {
            SelectedTask = Tasks.SingleOrDefault(t => t.Id == id);
            TasksUpdated?.Invoke(this, null);
        }

        private async void LoadTasks()
        {
            Tasks = (await GetAllTasks()).Payload.ToList();
            TasksUpdated?.Invoke(this, null);
        }

        private async Task<GetAllTasksQueryResult> GetAllTasks()
        {
            return await httpClient.GetJsonAsync<GetAllTasksQueryResult>("tasks");
        }


        public void ToggleTask(Guid id)
        {
            foreach (var taskModel in Tasks)
            {
                if (taskModel.Id == id)
                {
                    taskModel.IsComplete = !taskModel.IsComplete;
                    TaskVm t = new TaskVm()
                    {
                        Id = id,
                        IsComplete = taskModel.IsComplete
                    };
                    Console.Write(t.Id);
                    UpdateTask(t);
                }
            }

            TasksUpdated?.Invoke(this, null);
        }

        private async Task<UpdateTaskCommandResult> Update(UpdateTaskCommand command)
        {
            return await httpClient.PutJsonAsync<UpdateTaskCommandResult>($"tasks/{command.Id}", command);
        }

        public async Task UpdateTask(TaskVm model)
        {
            var result = await Update(model.ToUpdateTaskCommand());
            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    Tasks = updatedList.ToList();
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
            }
        }


        private async Task<CreateTaskCommandResult> AddTask(CreateTaskCommand command)
        {

            return await httpClient.PostJsonAsync<CreateTaskCommandResult>("tasks", command);
        }

        public async Task CreateTask(TaskVm model)
        {
            var result = await AddTask(model.ToCreateTaskCommand());
            if (result != null)
            {
                     var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    Tasks = updatedList.ToList();
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
            }
        }

        public void LoadAllTasksByBtnClick()
        {
            showAllTasks = !showAllTasks;
        }

        private async Task<HttpResponseMessage> Delete(DeleteTaskCommand command)
        {
            return await httpClient.DeleteAsync($"tasks/{command.Id}");
        }

        public async Task DeleteTask(TaskVm model)
        {
            var result = await Delete(model.ToDeleteTaskCommand());
            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    Tasks = updatedList.ToList();
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
            }
        }



    }
}