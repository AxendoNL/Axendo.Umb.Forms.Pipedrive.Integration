function pipedriveResource($http, umbRequestHelper) {

    return {
        getAllPersonFields: function () {
            return umbRequestHelper.resourcePromise($http.get(umbRequestHelper.getApiUrl("umbracoFormsExtensionsPipedriveBaseUrl",
                                                                                         "GetAllPersonFields")),
                                                                                         'Failed to get Pipedrive Properties');
        },
    };
}

angular.module('umbraco.resources').factory('pipedriveResource', pipedriveResource);