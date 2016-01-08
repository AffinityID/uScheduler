angular.module("umbraco.resources")
    .factory("uSchedulerResource", function($http) {
        return {
            saveSchedule: function(data) {
                return $http.post("backoffice/uScheduler/SchedulerApi/SaveSchedule", JSON.stringify(data));
            },
            getSchedule: function(id) {
                return $http.get("backoffice/uScheduler/SchedulerApi/GetSchedule?id=" + id);
            },
            deleteSchedule: function(id) {
                return $http.delete("backoffice/uScheduler/SchedulerApi/DeleteSchedule?id=" + id);
            }
        };
    });