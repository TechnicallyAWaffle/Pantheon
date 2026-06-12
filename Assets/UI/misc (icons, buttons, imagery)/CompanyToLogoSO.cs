using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCompanyToLogo", menuName = "Company To Logo")]
public class CompanyToLogoSO : ScriptableObject
{
    public List<CompanyToLogo> companyToLogos = new();

    public Texture2D GetLogo(SOProcessData.Enterprise enterprise)
    {
        foreach (var kvp in companyToLogos)
        {
            if (kvp.enterprise == enterprise)
            {
                return kvp.logo;
            }
        }

        throw new KeyNotFoundException($"Couldn't find logo for enterprise: {enterprise}");
    }
}

[Serializable]
public struct CompanyToLogo
{
    public SOProcessData.Enterprise enterprise;
    public Texture2D logo;
}