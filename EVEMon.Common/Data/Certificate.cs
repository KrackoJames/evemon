﻿using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a certificate from a character's point of view.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class Certificate
    {
        private readonly Character m_character;
        private readonly CertificateLevel[] m_levels;


        #region Initialization, importation, exportation and update

        /// <summary>
        /// Constructor at character's initialization time.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        /// <param name="certClass"></param>
        internal Certificate(Character character, StaticCertificate src, CertificateClass certClass)
        {
            m_character = character;
            m_levels = new CertificateLevel[6];

            StaticData = src;
            Class = certClass;

            foreach (KeyValuePair<CertificateGrade, List<StaticSkillLevel>> skill in src.PrerequisiteSkills)
            {
                m_levels[(int)skill.Key] = new CertificateLevel(skill, this, character);
            }
        }

        #endregion


        #region Core properties

        /// <summary>
        /// Gets the static data associated with this certificate.
        /// </summary>
        public StaticCertificate StaticData { get; private set; }

        /// <summary>
        /// Gets this certificate's id.
        /// </summary>
        public int ID
        {
            get { return StaticData.ID; }
        }

        /// <summary>
        /// Gets this certificate's name.
        /// </summary>
        public string Name
        {
            get { return StaticData.Class.Name; }
        }

        /// <summary>
        /// Gets this certificate's description.
        /// </summary>
        public string Description
        {
            get { return StaticData.Description; }
        }

        /// <summary>
        /// Gets the class for this certificate.
        /// </summary>
        public CertificateClass Class { get; private set; }

        /// <summary>
        /// Gets the ships this certificate is recommended for.
        /// </summary>
        public IEnumerable<Item> Recommendations
        {
            get { return StaticData.Recommendations; }
        }

        /// <summary>
        /// Gets the level one of the cerificate.
        /// </summary>
        /// <value>
        /// The level one.
        /// </value>
        public CertificateLevel LevelOne
        {
            get { return m_levels[(int)CertificateGrade.Basic]; }
        }

        /// <summary>
        /// Gets the level two of the cerificate.
        /// </summary>
        /// <value>
        /// The level two.
        /// </value>
        public CertificateLevel LevelTwo
        {
            get { return m_levels[(int)CertificateGrade.Standard]; }
        }

        /// <summary>
        /// Gets the level three of the cerificate.
        /// </summary>
        /// <value>
        /// The level three.
        /// </value>
        public CertificateLevel LevelThree
        {
            get { return m_levels[(int)CertificateGrade.Improved]; }
        }

        /// <summary>
        /// Gets the level four of the cerificate.
        /// </summary>
        /// <value>
        /// The level four.
        /// </value>
        public CertificateLevel LevelFour
        {
            get { return m_levels[(int)CertificateGrade.Advanced]; }
        }

        /// <summary>
        /// Gets the level five of the cerificate.
        /// </summary>
        /// <value>
        /// The level five.
        /// </value>
        public CertificateLevel LevelFive
        {
            get { return m_levels[(int)CertificateGrade.Elite]; }
        }

        /// <summary>
        /// Gets all levels of the cerificate.
        /// </summary>
        /// <value>
        /// All level.
        /// </value>
        public IEnumerable<CertificateLevel> AllLevel
        {
            get { return m_levels.Where(level => level != null); }
        }

        #endregion


        #region Helper methods and properties  

        /// <summary>
        /// Try to update the certificate's status. 
        /// </summary>
        /// <returns>True if the status was updated, false otherwise.</returns>
        internal bool TryUpdateCertificateStatus()
        {
            return m_levels.Where(level => level != null)
                .Aggregate(false, (current, level) => current | level.TryUpdateCertificateStatus());
        }

        /// <summary>
        /// Gets all the top-level prerequisite skills.
        /// </summary>
        public IEnumerable<SkillLevel> AllTopPrerequisiteSkills
        {
            get { return StaticData.AllTopPrerequisiteSkills.ToCharacter(m_character); }
        }

        /// <summary>
        /// Gets the required training time for the provided character to train this certificate.
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTrainingTime
        {
            get { return m_character.GetTrainingTimeToMultipleSkills(AllTopPrerequisiteSkills); }
        }

        /// <summary>
        /// Gets the lowest untrained certificate level.
        /// Null if all certificates have been trained.
        /// </summary>
        public CertificateLevel LowestUntrainedLevel
        {
            get { return AllLevel.FirstOrDefault(cert => !cert.IsTrained); }
        }

        /// <summary>
        /// Gets the highest trained certificate level.
        /// Null if no certificates have been trained.
        /// </summary>
        public CertificateLevel HighestTrainedLevel
        {
            get { return AllLevel.LastOrDefault(cert => cert.IsTrained); }
        }

        #endregion


        /// <summary>
        /// Gets a string representation of this certificate.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StaticData.ToString();
        }

        /// <summary>
        /// Implicit conversion operator to the static equivalent of this certificate.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static implicit operator StaticCertificate(Certificate cert)
        {
            return cert == null ? null : cert.StaticData;
        }
    }
}