# Fhydia (Fluent Hy[perme]dia) Library

The goal of this library is to enrich your endpoints response with Hypermedia (HATEOAS) features with the least impact on your existing code.
The objective is to support multiple HATEOAS specifications.

# How ?

By describing in a fluent way your response objects relations and configuration.
When your response enter into the dedicated Filter, Fhydia read the configuration and depending on the accept-header provided in the request add or remove stuff dynamically onto your returned object (by transforming it to an ExpandoObject)

# TODO

## Parser

- [x] Basic controller parsing (Name, Group, Methods and returned type)

## Fluent Configuration

- [x] Create basic builders
- [x] Create basic configuration from builders

## Dedicated configuration
- [ ] FhydiaConfiguration<T> support (like EF) to keep your configuration separated

## Handlers

- [x] Response Filter to enrich the model with corresponding configuration
- [ ] Request header Media type support in order to choose the configuration to use (hal, hydra, jsonld etc)
- [ ] Authorization/Visibility support (to show/hide properties or links depending on role)

## Transformers

- [ ] HAL
- [ ] JSON-API
- [ ] COLLECTION-JSON
- [ ] HYDRA
- [ ] JSON-LD
- [ ] HAL-FORMS
- [ ] SIREN
- [ ] UBER
- [ ] MASON