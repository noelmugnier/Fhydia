# Fhydia (Fluent Hypermedia) Library
The goal of this library is to augment your endpoints response with Hypermedia (HATEOAS) features with a minimum impact on your existing code.
The objective is to support JSON-LD, HAL, SIREN and HYDRA specifications.

# How ?
By parsing your controller/endpoints and generate a DSL in order to transform your responses according to your needs. This DSL try to infer a lot of configurations to avoid manual stuff, but if you need more control you can fluently describe your endpoints, models and links.
So when your response enter into our dedicated Filter, Fhydia read the configured DSL and (depending on the accept-header provided in the request) add or remove stuff dynamically onto your returned object (by transforming it to an Expando)

# TODO
## Parser
- [x] Basic controller parsing (Name, Group, Methods with basic url)
- [x] Basic parsing of attributes (HttpAttributes, RouteAttributes, Description, NonController...)
- [x] Basic controller inheritance support
- [ ] Support of [controller] and [action] placeholder while parsing template uri
- [ ] Presence or absence of FromXXXAttribute on method params (with inferences)
- [ ] Handle Swashbuckle SwaggerResponse and SwaggerOperation Attributes
- [ ] Parsing AcceptVerbsAttribute
- [ ] Parsing of ConsumesAttribute (not scheduled)
- [ ] Namespace convention routing (not scheduled)

## DSL Engine
- [ ] Build endpoints from parsed controller data
- [ ] Aggregate returned type to endpoints to facilitate links generation

## Fluent Configuration
- [ ] AddFhydia extension wrapper to configure API
- [ ] UseFhydia extension wrapper
- [ ] FhydiaConfiguration<T> support (like EF) to keep your configuration separated

## Transformers
- [ ] JSON-LD
- [ ] HAL
- [ ] HAL-Forms
- [ ] SIREN
- [ ] HYDRA

## Handlers
- [ ] Header Filter to parse request header in order to choose the configuration to use (hal, hydra, jsonld etc)
- [ ] Response Filter to augment the model with corresponding configuration
- [ ] Auth/Visibility pipeline support (to show/hide properties or links depending on role)
- [ ] OpenApi3 Links support for swashbuckle and other lib ?
