---
title: How I use Ollama in .NET projects
type: article
projects: [real-estate, learn-language]
description: The clean way to use local hosted models available in Ollama with .NET
createdAt: 2025-12-26
updatedAt: 2025-12-26
---
## Why to use ollama

Nowadays, AI is used in a lot of applications. And your next application also may require it.
There are a lot of AI providers can be used, but some applications may have features that don’t allow to use them. 
The reasons may be:
1. **Application contains sensitive data:** such applications can be forbidden to send data to external providers. 
2. **Application requires reproducability:** provider models can be changed, turned off. Sometimes business can't take 
such risks and uses his own models.
3. **Export regulations:** You can be located in the country where providers don’t work.
Or your tasks require little models but makes a lot of requests. Or they should work without internet.

In any of these cases you will need to use locally hosted models. Teaching your own can steal a lot of time,
and I prefer to use ready models for different MVPs.

Ollama provides universal API to use different models from a code. It lets to switch models in a few seconds if
requirements are changed. When the project leaves the MVP stage, ollama calls can be replaced to
the specific fine-tuned model.

- Advantages and disadvantages

** universality. API is compatible with ChatGpt etc, so ollama can be used in local instances and be replaced in production


- Real cases
- Native usage
- Laraue adapter