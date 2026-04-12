---
layout: post
title: Naming Tests - Quick Note
date: 2026-04-12
tags: testing, naming, programming, unit-tests
categories: programming
---

Tests should read like plain English descriptions of behavior, not implementation details. Good naming: `Delivery_with_a_past_date_is_invalid`. Bad naming: `IsDeliveryValid_InvalidDate_ReturnsFalse`. See [Vladimir Khorikov's post on naming tests](https://enterprisecraftsmanship.com/posts/you-naming-tests-wrong/).
