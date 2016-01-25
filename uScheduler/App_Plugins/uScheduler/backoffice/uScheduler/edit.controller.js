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

            function removeHeader(header) {
                var headers = $scope.headers;
                headers.splice(headers.indexOf(header), 1);
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

            function loadOptionValues() {
                $scope.schedule = {
                    HttpVerb: 'GET',
                    Frequency: 'Single'
                };

                resource.getScheduleHttpVerbs()
                   .then(function (response) {
                       $scope.verbs = response.data;
                   });

                resource.getScheduleFrequencies()
                    .then(function (response) {
                        $scope.frequencies = response.data;
                    });
            }

            function loadHeaders() {
                var headers = $scope.schedule.Headers;
                if (headers) {
                    Object.keys(headers).forEach(function (key) {
                        var header = {
                            key: key,
                            value: headers[key]
                        }
                        $scope.headers.push(header);
                    });
                }

                $scope.headers.push({});
            }

            function watchHeaders() {
                $scope.headers = [];
                $scope.$watch('headers', function (newValue) {
                    $scope.schedule.Headers = {};
                    $scope.headers.forEach(function (h) {
                        if (!h.key || !h.value) return;

                        $scope.schedule.Headers[h.key] = h.value;
                    });

                    var lastHeader = newValue[newValue.length - 1];
                    if (lastHeader && lastHeader.key && lastHeader.value) {
                        $scope.headers.push({});
                    }
                }, true);
            }

            function initialize() {
                var id = $route.id;
                $scope.isNew = id <= -1;
                syncTree(id);
                bindDatePicker();
                watchHeaders();
                loadOptionValues();

                if ($scope.isNew) {
                    $scope.loaded = true;
                } else {
                    resource.getSchedule(id)
                        .then(function (response) {
                            $scope.schedule = response.data;
                            loadHeaders();
                            $scope.loaded = true;
                        });

                    resource.getScheduleLogs(id, 1, 10)
                        .then(function(response) {
                            $scope.logs = response.data;
                        });
                }
            }

            $scope.toggleStatus = toggleStatus;
            $scope.removeHeader = removeHeader;
            $scope.save = save;
            $scope.run = run;
            initialize();
        }
    ]);