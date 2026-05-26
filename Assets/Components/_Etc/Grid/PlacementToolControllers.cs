using UnityEngine;

public interface IPlacementToolHost
{
    bool TryGetPlacementPoint(out Vector3 position, out Collider hitCollider);
    bool TryGetToolTargetHit(out RaycastHit hit);
    void SetCurrentPlacementTargetCollider(Collider hitCollider);
    bool ShouldRefreshPlacePreview(Vector3 position);
    int GetSelectedPlacementObjectId();
    bool CheckPlacementValidity(Vector3 worldPosition, int objectId);
    Quaternion GetCurrentPlacementRotation();
    void SetMouseIndicatorPosition(Vector3 worldPosition);
    void UpdatePreviewPosition(Vector3 worldPosition, bool isValid, Quaternion rotation);
    void EnsurePreviewForCurrentToolMode();
    bool HasPendingMoveObject();
    bool CheckPendingMovePlacementValidity(Vector3 worldPosition, Quaternion rotation);
    void MarkPlacementPreviewClean();
    void HandlePlaceAction(Vector3 worldPosition);
    void HandleDeleteAction(Collider hitCollider);
    void HandleMoveAction(Vector3 worldPosition, Collider hitCollider);
}

public interface IPlacementToolController
{
    void UpdateTool();
    void HandlePrimaryAction();
}

public class PlaceToolController : IPlacementToolController
{
    private readonly IPlacementToolHost host;

    public PlaceToolController(IPlacementToolHost host)
    {
        this.host = host;
    }

    public void UpdateTool()
    {
        if (!host.TryGetPlacementPoint(out var position, out var hitCollider))
        {
            return;
        }

        host.SetCurrentPlacementTargetCollider(hitCollider);
        if (!host.ShouldRefreshPlacePreview(position))
        {
            return;
        }

        int selectedObjectId = host.GetSelectedPlacementObjectId();
        if (selectedObjectId < 0)
        {
            return;
        }

        bool isValid = host.CheckPlacementValidity(position, selectedObjectId);
        Quaternion rotation = host.GetCurrentPlacementRotation();
        host.SetMouseIndicatorPosition(position);
        host.UpdatePreviewPosition(position, isValid, rotation);
    }

    public void HandlePrimaryAction()
    {
        if (!host.TryGetPlacementPoint(out var position, out var hitCollider))
        {
            return;
        }

        host.SetCurrentPlacementTargetCollider(hitCollider);
        host.HandlePlaceAction(position);
    }
}

public class DeleteToolController : IPlacementToolController
{
    private readonly IPlacementToolHost host;

    public DeleteToolController(IPlacementToolHost host)
    {
        this.host = host;
    }

    public void UpdateTool()
    {
        host.EnsurePreviewForCurrentToolMode();
        if (host.TryGetToolTargetHit(out var hit))
        {
            host.SetMouseIndicatorPosition(hit.point);
        }
    }

    public void HandlePrimaryAction()
    {
        if (host.TryGetToolTargetHit(out var hit))
        {
            host.HandleDeleteAction(hit.collider);
        }
    }
}

public class MoveToolController : IPlacementToolController
{
    private readonly IPlacementToolHost host;

    public MoveToolController(IPlacementToolHost host)
    {
        this.host = host;
    }

    public void UpdateTool()
    {
        host.EnsurePreviewForCurrentToolMode();

        if (!host.TryGetPlacementPoint(out var position, out var hitCollider))
        {
            if (host.TryGetToolTargetHit(out var toolHit))
            {
                host.SetMouseIndicatorPosition(toolHit.point);
            }
            return;
        }

        host.SetCurrentPlacementTargetCollider(hitCollider);
        Quaternion rotation = host.GetCurrentPlacementRotation();
        bool isValid = !host.HasPendingMoveObject() || host.CheckPendingMovePlacementValidity(position, rotation);

        host.SetMouseIndicatorPosition(position);
        host.UpdatePreviewPosition(position, isValid, rotation);
        host.MarkPlacementPreviewClean();
    }

    public void HandlePrimaryAction()
    {
        if (!host.HasPendingMoveObject())
        {
            if (host.TryGetToolTargetHit(out var pickHit))
            {
                host.HandleMoveAction(Vector3.zero, pickHit.collider);
            }
            return;
        }

        if (!host.TryGetPlacementPoint(out var position, out var hitCollider))
        {
            return;
        }

        host.SetCurrentPlacementTargetCollider(hitCollider);
        host.HandleMoveAction(position, hitCollider);
    }
}
