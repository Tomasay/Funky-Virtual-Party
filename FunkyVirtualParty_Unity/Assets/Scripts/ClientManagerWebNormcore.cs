using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ClientManagerWebNormcore : MonoBehaviour
{
    [SerializeField]
    private RealtimeAvatarManager _manager;

    private void Awake()
    {
        _manager = GetComponent<RealtimeAvatarManager>();
        _manager.avatarCreated += AvatarCreated;
        _manager.avatarDestroyed += AvatarDestroyed;
    }

    private void AvatarCreated(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if(isLocalAvatar)
        {

        }
    }

    private void AvatarDestroyed(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        // Avatar destroyed!
    }
}
