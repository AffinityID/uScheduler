angular.module('umbraco.resources')
    .factory('uScheduler.Resource', function($http) {
        return {
            getScheduleHttpVerbs: function() {
                return $http.get('backoffice/uScheduler/SchedulerApi/GetScheduleHttpVerbs');
            },
            getScheduleFrequencies: function() {
                return $http.get('backoffice/uScheduler/SchedulerApi/GetScheduleFrequencies');
            },
            getSchedule: function (id) {
                return $http.get('backoffice/uScheduler/SchedulerApi/GetSchedule?id=' + id);
            },
            saveSchedule: function (data) {
                return $http.post('backoffice/uScheduler/SchedulerApi/SaveSchedule', JSON.stringify(data));
            },
            runSchedule: function(id) {
                return $http.post('backoffice/uScheduler/SchedulerApi/RunSchedule?id=' + id);
            },
            deleteSchedule: function(id) {
                return $http.delete('backoffice/uScheduler/SchedulerApi/DeleteSchedule?id=' + id);
            },
            getScheduleLogs: function(id, page, itemsPerPage) {
                return $http.get('backoffice/uScheduler/SchedulerApi/GetLogs?scheduleId=' + id + '&page=' + page + '&itemsPerPage=' + itemsPerPage);
            }
        };
    });