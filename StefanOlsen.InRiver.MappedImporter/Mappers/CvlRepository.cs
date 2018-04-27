/*
 * Copyright (c) 2018 Stefan Holm Olsen
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

﻿using System.Collections.Generic;
using System.Linq;
using inRiver.Remoting;
using inRiver.Remoting.Objects;

// ReSharper disable InconsistentNaming
namespace StefanOlsen.InRiver.MappedImporter.Mappers
{
    public class CvlRepository
    {
        private readonly IinRiverManager _inRiverManager;

        private readonly Dictionary<string, CVL> _cvls = new Dictionary<string, CVL>();
        private readonly Dictionary<string, Dictionary<string, CVLValue>> _cvlValues =
            new Dictionary<string, Dictionary<string, CVLValue>>();

        public CvlRepository(IinRiverManager inRiverManager)
        {
            _inRiverManager = inRiverManager;
        }

        public CVL AddCVL(CVL cvl)
        {
            if (_cvls.TryGetValue(cvl.Id, out CVL cachedCvl))
            {
                return cachedCvl;
            }

            cvl = _inRiverManager.ModelService.AddCVL(cvl);
            _cvls.Add(cvl.Id, cvl);

            return cvl;
        }

        public CVLValue AddCVLValue(CVLValue cvlValue)
        {
            if (!_cvlValues.TryGetValue(cvlValue.CVLId, out Dictionary<string, CVLValue> cvlValues))
            {
                // Initialize list of CVL values.
                GetCVLValuesForCVL(cvlValue.CVLId);

                return cvlValue;
            }

            if (cvlValues.TryGetValue(cvlValue.Key, out CVLValue existingCvlValue))
            {
                return existingCvlValue;
            }

            cvlValue = _inRiverManager.ModelService.AddCVLValue(cvlValue);
            cvlValues.Add(cvlValue.Key, cvlValue);

            return cvlValue;
        }

        public ICollection<CVL> GetAllCVLs()
        {
            if (_cvls.Count != 0)
            {
                return _cvls.Values;
            }

            ICollection<CVL> cvls = _inRiverManager.ModelService.GetAllCVLs();
            foreach (var cvl in cvls)
            {
                _cvls.Add(cvl.Id, cvl);
            }

            return _cvls.Values;
        }

        public CVL GetCVL(string cvlId)
        {
            if (_cvls.Count == 0)
            {
                // If not found, preload all CVLs.
                GetAllCVLs();
            }

            return _cvls.TryGetValue(cvlId, out CVL cvl)
                ? cvl
                : null;
        }

        public CVLValue GetCVLValueByKey(string cvlId, string key)
        {
            if (!_cvlValues.TryGetValue(cvlId, out Dictionary<string, CVLValue> cvlValues))
            {
                cvlValues = _inRiverManager.ModelService.GetCVLValuesForCVL(cvlId)
                    .ToDictionary(cv => cv.Key, cv => cv);

                _cvlValues.Add(cvlId, cvlValues);
            }

            return cvlValues.TryGetValue(key, out CVLValue cvlValue)
                ? cvlValue
                : null;
        }

        public ICollection<CVLValue> GetCVLValuesForCVL(string cvlId)
        {
            if (_cvlValues.TryGetValue(cvlId, out Dictionary<string, CVLValue> cvlValues))
            {
                return cvlValues.Values.ToArray();
            }

            cvlValues = _inRiverManager.ModelService.GetCVLValuesForCVL(cvlId)
                .ToDictionary(cv => cv.Key, cv => cv);

            _cvlValues.Add(cvlId, cvlValues);

            return cvlValues.Values.ToArray();
        }
    }
}
