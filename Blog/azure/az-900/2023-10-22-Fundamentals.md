# Azure Portal
# Azure CLI
# Azure PowerShell
# Cloud SHell
# ARM template
# Azure Advisor

# Cloud concepts

- high availability
- reliability
- scalability (up, out)
- predictability (performance, cost)
- security
- governance
- manageability

# CapEx and OpEx

- costs for computing
- CapEx cost to buy hardware
- OpEx is ongoing cost

# IaaS, PaaS, SaaS
# Shared Responsibility Model
# Azure Marketplace
# Cloud Models

- private
- public
- hybrid

# Azure Architecture

- Regions
- Availability Zones
- Resources Groups
- Resource Manager

# Compute

- virtual machines (IaaS)
- scale sets (group of VMs)
- app services (PaaS)
    - web apps
    - web apps for containers
    - api app
- azure container instances
- azure kubernetes service
- azure container registry (image storage)
- virtual desktop
- functions

# Networking

- virtual network
    - subnet
    - peering
- load balancer
- VPN gateway
- application gateway
- express route
    - specific connection to azure (no via internet)
- CDN

# Storage

- Blob
    - Storage account (unique address)
    - Account has contains where all blobs saved
    - Types:
        - block
        - append
        - page
    - pricing tier
        - hot
        - cool
        - archive
- Disk
    - types:
        - hdd
        - Standard SSD
        - Premium SSD
        - Ultra Disk
- File storage
    - sharing
    - managed
    - resilient
- Archive
- Storage Redundancy
    - single region
        - locally redundant storage (LRS) (3 copies in one AZ and single datacenter)
        - zone-redundant storage (ZRS) (3 copies - one per AZ)
    - multi-region
        - geo-redundant storage (3 copies in primary AZ and 3 copies in another region AZ)
        - geo-zone redundant storage (like ZRS in primary region and LRS in secondary region)
    - all options has three copies in primray region and three in secondary (multi-region types)
- Moving Data
    - AzCopy
    - Storage Explorer
    - Azure File Sync
- Migration options
    - Azure Data Box (lots data movement)
    - Azure Migrate service
- Performance options
    - standard
    - premium
        - blocks blobs (LRS/ZRS only)
        - page blobs (LRS only)
        - file shares (LRS/ZRS only)

# Database

- Cosmos DB
- Azure SQL
- Azure Database for MySQL
- Azure Database for PostgreSQL
- Database migration services

# Authentication and Authorization

- identity services
- azure active directory
    - tenant
        - represents organization
        - a tenant is a dedicated instance of AAD that an organization receives when signing up for Azure
        - each user can be a member or guest of up to 500 AAD tenants
    - subcription
        - all resource whithin a subscription are billed together
        - it is possible to have multiple subscriptions within a tenant to separate costs
- zero trust concepts
- multi-factor authentication
- conditional access
- passwordless authentication
- external guest access
- azure active directory domain services
- single sign-on

# Azure Solutions

- IOT
    - Hub (PaaS)
    - Central (SaaS)
    - Sphere
- Big Data
    - Data Lake Analytics
    - Databricks
    - Synapse Analytics
- Machine Learning
    - Azure Bot Service
    - Azure Cognitive Services
- Serverless
    - Functions
    - Logic Apps
    - Event Grid
- DevOps
    - DevTest Labs
