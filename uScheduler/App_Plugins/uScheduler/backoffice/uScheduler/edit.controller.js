angular.module("umbraco")
    .controller("uScheduler.EditController", ['$scope', '$routeParams', '$element',
        'navigationService', 'treeService', 'notificationsService', 'contentEditingHelper', 'uScheduler.Resource',
        function($scope, $route, $element, nav, tree, notification, editHelper, resource) {
            function toggleStatus() {
                $scope.schedule.Disabled = $scope.schedule.Disabled ? false : true;
                save();
            }

            function syncTree(id) {
                nav.syncTree({ tree: 'uScheduler', path: [id.toString()], forceReload: true, activate: true });
            }

            function save() {
                //Retrieve the set NextRun datetime from the element as the Umbraco control breaks the link from the passed in model.
                $scope.schedule.NextRun = $('#datepickernextRun').data().date;

                resource.saveSchedule($scope.schedule).then(function (response) {
                    $scope.schedule = response.data;
                    var id = response.data.Id;
                    syncTree(id);
                    editHelper.redirectToCreatedContent(id);
                    notification.success(response.data.Name + ' has been saved.');
                });
            }

            function run() {
                resource.runSchedule($route.id);
            }

            function initialize() {
                var id = $route.id;
                $scope.isNew = id <= -1;

                $scope.datepicker = {
                    editor: "Umbraco.DateTime",
                    label: 'Next Run',
                    description: 'Next scheduled invocation.',
                    hideLabel: false,
                    view: "datepicker",
                    alias: 'nextRun',
                    validation: {
                        mandatory: true
                    },
                    config: {
                        format: "YYYY-MM-DD HH:mm",
                        useSeconds: false
                    }
                };

                resource.getScheduleHttpVerbs()
                   .then(function (response) {
                       $scope.verbs = response.data;
                   });

                resource.getScheduleFrequencies()
                    .then(function(response) {
                        $scope.frequencies = response.data;
                    });

                if ($scope.isNew){
                    $scope.schedule = {
                        HttpVerb: 'GET',
                        Frequency: 'Single'
                    };
                    $scope.loaded = true;
                } else {
                    resource.getSchedule($route.id)
                        .then(function (response) {
                            $scope.schedule = response.data;
                            $scope.datepicker.value = $scope.schedule.NextRun;
                            $scope.loaded = true;
                        });
                }
            }

            $scope.toggleStatus = toggleStatus;
            $scope.save = save;
            $scope.run = run;
            initialize();
        }
    ]);