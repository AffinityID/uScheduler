angular.module("umbraco")
    .controller("uScheduler.DeleteController", ['$scope', '$location', '$routeParams', 'navigationService', 'treeService', 'notificationsService', 'uScheduler.Resource',
        function($scope, $location, $route, nav, tree, notification, resource) {
            $scope.delete = function(id) {
                resource.deleteSchedule(id)
                    .then(function () {
                        tree.removeNode($scope.currentNode);
                        nav.hideNavigation();
                        notification.error($scope.currentNode.name + ' has been deleted.');

                        if ($route.id === id) {
                            $location.path("/uScheduler");
                        }
                    });
            };

            $scope.cancel = function() {
                nav.hideNavigation();
            };
        }
    ]);