---
layout: post
title: "Understanding Docker"
date: 2017-01-21

tags: docker
categories: deployment
---
### [What is Docker?](https://www.docker.com/what-docker)

Docker is the world's leading software containerization platform

> Note: Docker is licensed under the open source [Apache 2.0](https://inteist.com/how-to-use-apache-2-0-in-commercial-products-explained-in-simple-terms/) license.

Docker containers wrap a piece of software in a complete filesystem that contains everything needed to run: code, runtime, system tools, system libraries – anything that can be installed on a server. This guarantees that the software will always run the same, regardless of its environment.

![image1](./images/WhatIsDocker_1_kernal-2_1.png)

1. Lightweight

Containers running on a single machine share the same operating system kernel; they start instantly and use less RAM. Images are constructed from layered filesystems and share common files, making disk usage and image downloads much more efficient.

2. Open and supported by community

Docker containers are based on open standards, enabling containers to run on all major Linux distributions and on Microsoft Windows - and on top of any infrastructure. In addition, because Docker's [partnering](http://www.zdnet.com/article/what-is-docker-and-why-is-it-so-darn-popular/) with the other container powers, including Canonical, Google, Red Hat, and Parallels, on its key open-source component [runc](https://github.com/opencontainers/runc), it's brought standardization to containers. More info is [here](https://www.opencontainers.org/).

3. Secure by default

Containers isolate applications from one another and the underlying infrastructure, while providing an added layer of protection for the application.

### Containers and virtual machines
Containers and virtual machines have similar resource isolation and allocation benefits - but a different architectural approach allows containers to be more portable and efficient. Docker is built on top of [LXC](https://en.wikipedia.org/wiki/LXC). Like with any container technology, as far as the program is concerned, it has its own file system, storage, CPU, RAM, and so on. The [key difference](http://www.zdnet.com/article/what-is-docker-and-why-is-it-so-darn-popular/) between containers and VMs is that while the hypervisor abstracts an entire device, containers just abstract the operating system kernel. It means that you are able to run only containers which are supported by host operating system, i.e. there are containers for Windows OS and containers for Linux.

* Virtual Machines

![image](./images/WhatIsDocker_2_VMs_0-2_2.png)

Virtual machines include the application, the necessary binaries and libraries, and an entire guest operating system - all of which can amount to tens of GBs.

* Containers

![image](./images/WhatIsDocker_3_Containers_2_0.png)

Containers include the application and all of its dependencies - but share the kernel with other containers, running as isolated processes in user space on the host operating system. Docker containers are not tied to any specific infrastructure: they run on any computer, on any infrastructure, and in any cloud.

### Additional benefits
Docker allows you to dynamically change your application - from adding new capabilities and scaling services, to quickly changing problem areas.

* Shorter delivery time

On average, Docker users ship 7X more software after deploying Docker in their environment. More frequent software updates provide added value to customers.

* Quickly scale

Docker containers spin up and down in seconds, making it easy to scale application services to satisfy peak customer demand, and then reduce running containers when demand ebbs. And Docker containers are easy to deploy in a cloud.

* Easily remediate issue

Docker makes it easy to identify issues, isolate the problem container, quickly roll back to make the necessary changes, and then push the updated container into production. Isolation between containers makes these changes less disruptive than in traditional software models.

In summary: docker allows to deploy, use and support containers on production easier and safer than previous approaches. On the other hand, developers can use Docker to pack, ship, and run any application as a lightweight, portable, self sufficient LXC container that can run virtually anywhere.

### [Docker toolset](https://docs.docker.com/engine/understanding-docker/#/what-is-the-docker-platform)

Docker provides tooling and a platform to manage the lifecycle of your containers:

* Encapsulate your applications (and supporting components) into Docker containers
* Distribute and ship those containers to your teams for further development and testing
* Deploy those applications to your production environment, whether it is in a local data center or the Cloud

### [Docker Engine](https://docs.docker.com/engine/understanding-docker/#/what-is-docker-engine)
Docker Engine is a client-server application with these major components:

* A server which is a type of long-running program called a daemon process.
* A REST API which specifies interfaces that programs can use to talk to the daemon and instruct it what to do.
* A command line interface (CLI) client.

![image](./images/engine-components-flow.png)

### [What can I use Docker for?](https://docs.docker.com/engine/understanding-docker/#/what-can-i-use-docker-for)

Docker can streamline the development lifecycle by allowing developers to work in standardized environments using local containers which provide your applications and services. You can also integrate Docker into your continuous integration and continuous deployment (CI/CD) workflow.

Docker’s portability and lightweight nature also make it easy to dynamically manage workloads, scaling up or tearing down applications and services as business needs dictate, in near real time.

Docker is lightweight and fast. It provides a viable, cost-effective alternative to hypervisor-based virtual machines, allowing you to use more of your compute capacity to achieve your business goals.

### [Docker’s architecture](https://docs.docker.com/engine/understanding-docker/#/what-is-dockers-architecture)

Docker uses a client-server architecture. The Docker client and daemon communicate using a REST API, over UNIX sockets or a network interface. One client can even communicate with multiple unrelated daemons.

![image](./images/architecture.svg)

### [Inside Docker](https://docs.docker.com/engine/understanding-docker/#/inside-docker)

* Docker images

A Docker image is a read-only template with instructions for creating a Docker container. For example, an image might contain an [Nano Server](https://technet.microsoft.com/en-us/windows-server-docs/get-started/getting-started-with-nano-server) operating system with dotnet core and your web application installed. You can build or update images from scratch or download and use images created by others. A docker image is described in text file called a Dockerfile, which has a simple, well-defined syntax.

#### Docker images are the build component of Docker.

* Docker containers

A Docker container is a runnable instance of a Docker image. You can run, start, stop, move, or delete a container using Docker API or CLI commands. When you run a container, you can provide configuration metadata such as networking information or environment variables. Each container is an isolated and secure application platform, but can be given access to resources running in a different host or container, as well as persistent storage or databases.

#### Docker containers are the run component of Docker.

* Docker registries

A docker registry is a library of images. A registry can be public or private, and can be on the same server as the Docker daemon or Docker client, or on a totally separate server.

#### Docker registries are the distribution component of Docker.

* Docker services

A Docker service allows a swarm of Docker nodes to work together, running a defined number of instances of a replica task, which is itself a Docker image. You can specify the number of concurrent replica tasks to run, and the swarm manager ensures that the load is spread evenly across the worker nodes. To the consumer, the Docker service appears to be a single application.

#### Docker services are the scalability component of Docker.

### [Docker image work](https://docs.docker.com/engine/understanding-docker/#/how-does-a-docker-image-work)

Docker images are read-only templates from which Docker containers are instantiated. Each image consists of a series of layers. Docker uses union file systems to combine these layers into a single image. Union file systems allow files and directories of separate file systems, known as branches, to be transparently overlaid, forming a single coherent file system.

These layers are one of the reasons Docker is so lightweight. When you change a Docker image, such as when you update an application to a new version, a new layer is built and replaces only the layer it updates. The other layers remain intact. To distribute the update, you only need to transfer the updated layer. Layering speeds up distribution of Docker images. Docker determines which layers need to be updated at runtime.

An image is defined in a Dockerfile. Every image starts from a base image, such as ubuntu (Note: [Docker Hub](https://hub.docker.com/) is a public registry and stores images). The base image is defined using the FROM keyword in the dockerfile. There are a set of [intructions](https://docs.docker.com/engine/reference/builder/#/parser-directives) after this world usually. Each instruction creates a new layer in the image:
* Specify the base image ([FROM](https://docs.docker.com/engine/reference/builder/#/from))
* Specify the maintainer ([LABEL](https://docs.docker.com/engine/reference/builder/#/label))
* Run a command ([RUN](https://docs.docker.com/engine/reference/builder/#/run))
* Add a file or directory ([ADD](https://docs.docker.com/engine/reference/builder/#/add))
* Create an environment variable ([ENV](https://docs.docker.com/engine/reference/builder/#/env))
* What process to run when launching a container from this image ([CMD](https://docs.docker.com/engine/reference/builder/#/cmd))

Docker reads this Dockerfile when you request a build of an image, executes the instructions, and returns the image.

### [Docker registry work](https://docs.docker.com/engine/understanding-docker/#/how-does-a-docker-registry-work)

A Docker registry stores Docker images. After you build a Docker image, you can push it to a public registry such as Docker Hub or to a private registry running behind your firewall. You can also search for existing images and pull them from the registry to a host.

### [Container work](https://docs.docker.com/engine/understanding-docker/#/how-does-a-container-work)

A container uses the host machine’s Linux/Windows kernel, and consists of any extra files you add when the image is created, along with metadata associated with the container at creation or when the container is started. Each container is built from an image. The image defines the container’s contents, which process to run when the container is launched, and a variety of other configuration details. The Docker image is read-only. When Docker runs a container from an image, it adds a read-write layer on top of the image in which your application runs.

#### What happens when you run a container?

When you use the docker run CLI command or the equivalent API, the Docker Engine client instructs the Docker daemon to run a container. For example:

```docker
$ docker run -i -t ubuntu /bin/bash
```

When you run this command, Docker Engine does the following:

* **Pulls the ubuntu image**: Docker Engine checks for the presence of the ubuntu image. If the image already exists locally, Docker Engine uses it for the new container. Otherwise, then Docker Engine pulls it from Docker Hub.
* **Creates a new container**: Docker uses the image to create a container.
* **Allocates a filesystem and mounts a read-write layer**: The container is created in the file system and a read-write layer is added to the image.
* **Allocates a network / bridge interface**: Creates a network interface that allows the Docker container to talk to the local host.
* **Sets up an IP address**: Finds and attaches an available IP address from a pool.
* **Executes a process that you specify**: Executes the /bin/bash executable.
* **Captures and provides application output**: Connects and logs standard input, outputs and errors for you to see how your application is running, because you requested interactive mode.

You can manage and interact with it, use the services and applications it provides, and eventually stop and remove it.

### [The underlying technology](https://docs.docker.com/engine/understanding-docker/#/the-underlying-technology)

Docker is written in Go programming language.

#### [Namespaces](https://www.toptal.com/linux/separation-anxiety-isolating-your-system-with-linux-namespaces)
Docker uses a technology called namespaces to provide the isolated workspace called the container. When you run a container, Docker creates a set of namespaces for that container.

These namespaces provide a layer of isolation. Each aspect of a container runs in a separate namespace and its access is limited to that namespace.

Docker Engine uses namespaces such as the following on Linux:

* The `pid` namespace: Process isolation (PID: Process ID).
* The `net` namespace: Managing network interfaces (NET: Networking).
* The `ipc` namespace: Managing access to IPC resources (IPC: InterProcess Communication).
* The `mnt` namespace: Managing filesystem mount points (MNT: Mount).
* The `uts` namespace: Isolating kernel and version identifiers. (UTS: Unix Timesharing System).

#### [Control groups](https://en.wikipedia.org/wiki/Cgroups)
Docker Engine on Linux also relies on another technology called *control groups* (`cgroups`). A cgroup limits an application to a specific set of resources. Control groups allow Docker Engine to share available hardware resources to containers and optionally enforce limits and constraints. For example, you can limit the memory available to a specific container.

#### [Union file systems](https://en.wikipedia.org/wiki/UnionFS)
Union file systems, or UnionFS, are file systems that operate by creating layers, making them very lightweight and fast. Docker Engine uses UnionFS to provide the building blocks for containers.

#### [Container format](https://blog.docker.com/2015/07/open-container-format-progress-report/)
Docker Engine combines the namespaces, control groups, and UnionFS into a wrapper called a container format.

Resourses:
* [What is Docker?](https://www.docker.com/what-docker)
* [What is Docker and why is it so darn popular?](http://www.zdnet.com/article/what-is-docker-and-why-is-it-so-darn-popular/)
* [Понимая Docker](https://habrahabr.ru/post/253877/)
* [Docker Overview](https://docs.docker.com/engine/understanding-docker/)
* [Docker libcontainer unifies Linux container powers](http://www.zdnet.com/article/docker-libcontainer-unifies-linux-container-powers/)
* [ASP.NET Community Standup - January 17th, 2017 - Messing with Docker](https://www.youtube.com/watch?v=4nviEODZlsA&list=PL0M0zPgJ3HSftTAAHttA3JQU4vOjXFquF&index=0)
* [Working with Windows Containers and Docker: The Basics](https://www.simple-talk.com/sysadmin/virtualization/working-windows-containers-docker-basics/)
* [Understanding Docker presentation](https://docs.google.com/presentation/d/1M-b0BGA57bczBUg3er4rQsBta0U1RQ5lahrY1cEhUew/edit?usp=sharing)
