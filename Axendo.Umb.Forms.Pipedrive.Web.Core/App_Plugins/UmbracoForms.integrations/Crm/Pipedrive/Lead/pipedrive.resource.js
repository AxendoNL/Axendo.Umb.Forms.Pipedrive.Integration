function pipedriveLeadResource($http, umbRequestHelper) {

    return {
        getAllLeadFields: function () {
            return umbRequestHelper.resourcePromise($http.get(umbRequestHelper.getApiUrl("umbracoFormsExtensionsPipedriveLeadBaseUrl",
                                                                                         "GetAllLeadFields")),
                                                                                         'Failed to get Pipedrive LeadFields');
        },
    };
}

angular.module('umbraco.resources').factory('pipedriveLeadResource', pipedriveLeadResource);