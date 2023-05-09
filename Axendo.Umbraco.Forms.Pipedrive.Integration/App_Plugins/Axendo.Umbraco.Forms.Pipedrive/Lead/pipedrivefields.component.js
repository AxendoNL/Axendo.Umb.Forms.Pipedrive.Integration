angular
    .module("umbraco")
    .component("umbFormsPipedriveLeadFields", {
        controller: PipedriveLeadFieldsController,
        controllerAs: "vm",
        templateUrl: "/App_Plugins/Axendo.Umbraco.Forms.Pipedrive/Lead/pipedrive-field-mapper-template.html",
        bindings: {
            setting: "<"
        },

    }
    );

function PipedriveLeadFieldsController($scope, $compile, $element, $routeParams,
    pipedriveLeadResource, pickerResource) {
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

            pipedriveLeadResource.getAllLeadFields().then(function (response) {
                vm.pipedriveLeadFields = response.map(x => {
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
            leadField: ""
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