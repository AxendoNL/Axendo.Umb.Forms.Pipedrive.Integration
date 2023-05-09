angular
    .module("umbraco")
    .component("umbFormsPipedrivePersonFields", {
        controller: PipedrivePersonFieldsController,
        controllerAs: "vm",
        templateUrl: "/App_Plugins/Axendo.Umbraco.Forms.Pipedrive/Person/pipedrive-field-mapper-template.html",
        bindings: {
            setting: "<"
        },

    }
    );

function PipedrivePersonFieldsController($scope, $compile, $element, $routeParams,
    pipedriveResource, pickerResource) {
    var vm = this;

    vm.$onInit = function () {
        if (!vm.setting.value) {
            vm.mappings = [];
        } else {
            vm.mappings = JSON.parse(vm.setting.value);
        }

        var formId = $routeParams.id;
        if (formId !== -1) {


            pickerResource.getAllFields(formId).then(function (response) {
                vm.fields = response.data;
            });

            pipedriveResource.getAllPersonFields().then(function (response) {
                vm.pipedriveFields = response.map(x => {
                    return {
                        value: x.key,
                        name: x.name,
                        
                    }
                });
            });
        }
    }

    vm.addMapping = function () {
        vm.mappings.push({
            formField: "",
            pipedriveField: ""
        });
    };

    vm.deleteMapping = function (index) {
        vm.mappings.splice(index, 1);
        vm.setting.value = JSON.stringify(vm.mappings);
    };

    vm.stringifyValue = function () {
        vm.setting.value = JSON.stringify(vm.mappings);
    };
}