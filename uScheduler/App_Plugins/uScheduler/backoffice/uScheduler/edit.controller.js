angular.module("umbraco")
    .controller("uScheduler.EditController", ['$scope', '$routeParams', '$element',
        'navigationService', 'assetsService', 'notificationsService', 'contentEditingHelper', 'uScheduler.Resource',
        function($scope, $route, $element, nav, assetsService, notification, editHelper, resource) {
            function toggleStatus() {
                $scope.schedule.Disabled = $scope.schedule.Disabled ? false : true;
                save();
            }

            function syncTree(id, reload) {
                nav.syncTree({ tree: 'uScheduler', path: ['-1', id.toString()], activate: true, forceReload: reload });
            }

            function save() {
                resource.saveSchedule($scope.schedule)
                    .then(function (response) {
                        $scope.schedule = response.data;
                        var id = response.data.Id;
                        syncTree(id, true);
                        editHelper.redirectToCreatedContent(id);
                        notification.success(response.data.Name + ' has been saved.');
                    });
            }

            function run() {
                resource.runSchedule($route.id);
            }

            function bindDatePicker() {
                assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css');
                assetsService.load([
                    "lib/moment/moment-with-locales.js",
                    "lib/datetimepicker/bootstrap-datetimepicker.js"
                ]).then(function() {
                    var datepicker = $('#next-run-datepicker', $element).datetimepicker({
                        format: "YYYY-MM-DD HH:mm",
                        useSeconds: false,
                        icons: {
                            time: "icon-time",
                            date: "icon-calendar",
                            up: "icon-chevron-up",
                            down: "icon-chevron-down"
                        }
                    }).on('dp.change', function (e) {
                        $scope.schedule.NextRunUtc = e.date.utc();
                    });

                    $scope.$watch('schedule.NextRunUtc', function (newValue) {
                        datepicker.data('DateTimePicker').setDate(new Date(newValue));
                    });
                });
            }

            function initialize() {
                var id = $route.id;
                $scope.isNew = id <= -1;
                syncTree(id);
                $scope.schedule = {
                    HttpVerb: 'GET',
                    Frequency: 'Single'
                };
                bindDatePicker();

                resource.getScheduleHttpVerbs()
                   .then(function (response) {
                       $scope.verbs = response.data;
                   });

                resource.getScheduleFrequencies()
                    .then(function(response) {
                        $scope.frequencies = response.data;
                    });

                if ($scope.isNew) {
                    $scope.loaded = true;
                } else {
                    resource.getSchedule(id)
                        .then(function (response) {
                            $scope.schedule = response.data;
                            $scope.loaded = true;
                        });

                    resource.getScheduleLogs(id, 1, 10)
                        .then(function(response) {
                            $scope.logs = response.data;
                        });
                }
            }

            $scope.toggleStatus = toggleStatus;
            $scope.save = save;
            $scope.run = run;
            initialize();
        }
    ]);