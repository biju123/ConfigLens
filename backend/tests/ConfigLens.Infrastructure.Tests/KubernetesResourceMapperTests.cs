using System.Text.Json;
using ConfigLens.Domain.Scan.Aks;
using ConfigLens.Infrastructure.Kubernetes;
using FluentAssertions;
using k8s.Models;
using Xunit;

namespace ConfigLens.Infrastructure.Tests;

public class KubernetesResourceMapperTests
{
    [Fact]
    public void MapDeployment_reads_replica_counts_image_and_first_container_resources()
    {
        var deployment = new V1Deployment
        {
            Metadata = new V1ObjectMeta { Name = "assessor-api-6f9c", NamespaceProperty = "assessor-prod", Labels = new Dictionary<string, string> { ["app"] = "assessor-api" } },
            Spec = new V1DeploymentSpec
            {
                Replicas = 3,
                Template = new V1PodTemplateSpec
                {
                    Spec = new V1PodSpec
                    {
                        Containers =
                        [
                            new V1Container
                            {
                                Image = "assessor.azurecr.io/assessor-api:1.4.0",
                                Resources = new V1ResourceRequirements
                                {
                                    Requests = new Dictionary<string, ResourceQuantity> { ["cpu"] = new("250m"), ["memory"] = new("256Mi") },
                                    Limits = new Dictionary<string, ResourceQuantity> { ["cpu"] = new("500m"), ["memory"] = new("512Mi") }
                                }
                            }
                        ]
                    }
                }
            },
            Status = new V1DeploymentStatus { Replicas = 3, ReadyReplicas = 2 }
        };

        var resource = KubernetesResourceMapper.MapDeployment(deployment);

        resource.Namespace.Should().Be("assessor-prod");
        resource.AppOrService.Should().Be("assessor-api");
        resource.ResourceType.Should().Be(AksResourceType.Deployment);
        resource.ResourceName.Should().Be("assessor-api-6f9c");
        resource.DesiredReplicas.Should().Be(3);
        resource.RunningReplicas.Should().Be(3);
        resource.ReadyReplicas.Should().Be(2);
        resource.CpuRequest.Should().Be("250m");
        resource.CpuLimit.Should().Be("500m");
        resource.MemoryRequest.Should().Be("256Mi");
        resource.MemoryLimit.Should().Be("512Mi");
        resource.ContainerCount.Should().Be(1);
        resource.Image.Should().Be("assessor.azurecr.io/assessor-api:1.4.0");
    }

    [Fact]
    public void MapDeployment_falls_back_to_the_resource_name_when_no_app_label_is_present()
    {
        var deployment = new V1Deployment
        {
            Metadata = new V1ObjectMeta { Name = "unlabeled", NamespaceProperty = "assessor-prod" },
            Spec = new V1DeploymentSpec { Template = new V1PodTemplateSpec { Spec = new V1PodSpec { Containers = [] } } }
        };

        KubernetesResourceMapper.MapDeployment(deployment).AppOrService.Should().Be("unlabeled");
    }

    [Fact]
    public void MapPod_sums_restart_counts_across_containers_and_reads_phase()
    {
        var pod = new V1Pod
        {
            Metadata = new V1ObjectMeta { Name = "assessor-api-6f9c-abc12", NamespaceProperty = "assessor-prod", Labels = new Dictionary<string, string> { ["app.kubernetes.io/name"] = "assessor-api" } },
            Spec = new V1PodSpec { Containers = [new V1Container { Image = "assessor.azurecr.io/assessor-api:1.4.0" }] },
            Status = new V1PodStatus
            {
                Phase = "Running",
                ContainerStatuses =
                [
                    new V1ContainerStatus { RestartCount = 4, Name = "api", Ready = true, Image = "x", ImageID = "x", State = new V1ContainerState() },
                    new V1ContainerStatus { RestartCount = 3, Name = "sidecar", Ready = true, Image = "x", ImageID = "x", State = new V1ContainerState() }
                ]
            }
        };

        var resource = KubernetesResourceMapper.MapPod(pod);

        resource.AppOrService.Should().Be("assessor-api");
        resource.PodStatus.Should().Be("Running");
        resource.RestartCount.Should().Be(7);
    }

    [Fact]
    public void MapService_joins_ports_and_reads_cluster_ip()
    {
        var service = new V1Service
        {
            Metadata = new V1ObjectMeta { Name = "assessor-api", NamespaceProperty = "assessor-prod" },
            Spec = new V1ServiceSpec
            {
                ClusterIP = "10.0.12.34",
                Ports = [new V1ServicePort { Port = 443, TargetPort = 8443, Protocol = "TCP" }]
            }
        };

        var resource = KubernetesResourceMapper.MapService(service);

        resource.ClusterIp.Should().Be("10.0.12.34");
        resource.Ports.Should().Be("443:8443/TCP");
    }

    [Fact]
    public void MapScaledObject_reads_replica_bounds_and_trigger_types_from_the_custom_resource_json()
    {
        using var document = JsonDocument.Parse("""
            {
              "metadata": { "name": "assessor-worker-scaler", "namespace": "assessor-prod" },
              "spec": {
                "scaleTargetRef": { "name": "assessor-worker" },
                "minReplicaCount": 1,
                "maxReplicaCount": 10,
                "triggers": [ { "type": "azure-servicebus" }, { "type": "cpu" } ]
              }
            }
            """);

        var resource = KubernetesResourceMapper.MapScaledObject(document.RootElement);

        resource.Namespace.Should().Be("assessor-prod");
        resource.ResourceName.Should().Be("assessor-worker-scaler");
        resource.AppOrService.Should().Be("assessor-worker");
        resource.ResourceType.Should().Be(AksResourceType.KedaScaledObject);
        resource.MinReplicas.Should().Be(1);
        resource.MaxReplicas.Should().Be(10);
        resource.CurrentMetricValue.Should().Be("azure-servicebus, cpu");
    }
}
