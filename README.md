# Fhydia (Fluent Hy[perme]dia) Library

The goal of this library is to enrich your endpoints response with Hypermedia (HATEOAS) features with the least impact on your existing code.
The objective is to support multiple HATEOAS specifications.

# How ?

By describing in a fluent way your response objects relations and configuration.
When your response enter into the dedicated Filter, Fhydia read the configuration and depending on the accept-header provided in the request add or remove stuff dynamically onto your returned object (by transforming it to an ExpandoObject)

# Progress

## Configuration

- [x] Fluent configuration
- [x] FhydiaConfiguration<T> support (like EF) to keep your configuration separated

## Response handling

- [x] Use IEndpointFilter to transform response as ExpandoObject if required
- [x] Request 'Accept' header support in order to choose the formatter to use (only HAL supported for now)
- [x] Handle nested objects by recursion
- [ ] Authorization/Visibility support (to show/hide properties or links depending on role)

## Required formatters

- [ ] HAL (in progress)
- [ ] JSON-LD
- [ ] COLLECTION-JSON
- [ ] HYDRA
- [ ] HAL-FORMS

## Other formatters

- [ ] JSON-API
- [ ] SIREN
- [ ] UBER
- [ ] MASON

## Tests

A lot to do !