# Fhydia (Fluent Hy[perme]dia) Library

The goal of this library is to enrich your endpoints response with Hypermedia (HATEOAS) features with the least impact on your existing code.
The objective is to support multiple HATEOAS specifications.

# How ?

By describing in a fluent way your response objects relations and configuration.
When your response enter into the dedicated Filter, Fhydia read the configuration and depending on the accept-header provided in the request add or remove stuff dynamically onto your returned object (by transforming it to an ExpandoObject)

# TODO

## Parser

- [x] Basic controller parsing (Name, Group, Methods and returned type) to use it in formatters

## Fluent Configuration

- [x] Create basic builders
- [x] Create basic configuration from builders

## Dedicated configuration
- [ ] FhydiaConfiguration<T> support (like EF) to keep your configuration separated

## Handlers

- [x] Response Filter to force model as ExpandoObject if needed
- [x] Request 'Accept' header support in order to choose the configuration to use (hal, jsonld, collection-json etc)
- [x] Format nested response objects by recursion
- [ ] Authorization/Visibility support (to show/hide properties or links depending on role)

## Formatters

- [ ] HAL
- [ ] JSON-LD
- [ ] COLLECTION-JSON
- [ ] HYDRA
- [ ] HAL-FORMS
- [ ] JSON-API
- [ ] SIREN
- [ ] UBER
- [ ] MASON