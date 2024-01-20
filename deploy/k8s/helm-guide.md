1. The Helm chart directory contains:

- **Directory charts** – Used for adding dependent charts. Empty by default.
- The **chart.yaml** file includes the name, description, and version number of your app. It can also include [various other metadata and requirements](https://v2.helm.sh/docs/developing_charts/#the-chart-yaml-file).
- **Directory templates** is where the manifests that make up your chart are placed. In the sample chart the deployment, service, and ingress can be used to run an instance of NGINX.
- **YAML file** – Formatting information for configuring the chart.

2. About `values.yaml` file
- Includes default values for your application that can be overridden when your chart is deployed. This includes things like port numbers, host names, docker image names etc.

There are three possible values for the pullPolicy:
- **IfNotPresent** – Downloads a new version of the image if one does not exist in the cluster.
- **Always** – Pulls the image on every restart or deployment.
- **Latest** – Pulls the most up-to-date version available.
