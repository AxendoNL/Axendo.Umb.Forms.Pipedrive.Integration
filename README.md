# Axendo.Umb.Forms.Pipedrive.Integration

This integration between Umbraco Forms and CRM Pipedrive provides custom workflow within Umbraco Forms, which allows form entries to be mapped to a Pipedrive person and lead record,
and stored within Pipedrive.

## Features

- Form entries mapped to Pipedrive person and lead record, and stored within Pipedrive
- Additional form entries that can be mapped to custom Pipedrive person and lead data fields.
- Email check that prevents adding an existing user as a new person record.
- Adding a new lead to that existing person in Pipedrive instead of creating a new person record.

## Prerequisites

Requires minimum versions of Umbraco:

- CMS:8.1.0
- Forms: 8.7.4

## How To Use

### Configuration

You can download the NuGet Package from https://www.nuget.org/packages/Axendo.Umb.Forms.Pipedrive.Integration.

To use this intregration, you need an API key, which you can access by registering a Pipedrive account and can create and copy the key via Company settings > Personal preferences > API.

You need to add the API Key to the Web.config file.

<add key="PipedriveApiKey" value="your key here" />

### Working with the Umbraco Forms/ Pipedrive integration

#### Workflow

You can configure the Umbraco Forms by adding the "Save Person to Pipedrive" workflow to the form.

![Workflow](Axendo.Umb.Forms.Pipedrive.Web.Core/img/Workflow.png?raw=true "Title")

#### Pipedrive PersonField mapping

You can configure the mappings between the FormField and the Pipedrive PersonField. It is possible to add custom person data fields in Pipedrive. These will also show up as
a personfield which you can map with the FormField.

![Personfield mapping](Axendo.Umb.Forms.Pipedrive.Web.Core/img/PersonFieldMapping.png?raw=true "Title")

#### Pipedrive LeadField mapping

You can configure the mappings between the FormField and the Pipedrive Leadfield. A title field is required in the form to add a lead in Pipedrive.
You need to map this with the title leadField. If you want to add custom data fields that are not person specific like "Message", you can better add this as a custom lead field. 

![Personfield mapping](Axendo.Umb.Forms.Pipedrive.Web.Core/img/LeadMapping.png?raw=true "Title")

#### Required form fields

Like I mentioned before when creating a Form, you are required to add a title field in order to add a lead to a specific person record.

![Example Form](Axendo.Umb.Forms.Pipedrive.Web.Core/img/RequiredFormField.png?raw=true "Title")

#### Form submission

When a form is submitted on the website, the workflow will execute and create a new person and lead record in your Pipedrive account, using the information mapped from the fields in the form submission.

##### Person Record

![PersonRecord](Axendo.Umb.Forms.Pipedrive.Web.Core/img/PersonRecord.png?raw=true "Title")

##### Lead Record

![LeadRecord](Axendo.Umb.Forms.Pipedrive.Web.Core/img/LeadRecord.png?raw=true "Title")



