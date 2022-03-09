using CramMods.NARFI.Filters;
using CramMods.STUMP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CramMods.STUMP.Types
{
    public class Variant
    {
        private string? _name;
        public string? Name { get => _name; set => _name = value; }

        public Variant() { }
        public Variant(string name) : this() => _name = name;

        private VariantType _type = VariantType.Normal;
        public VariantType Type { get => _type; set => _type = value; }

        private VariantSelectMode _select = VariantSelectMode.Multiple;
        public VariantSelectMode Select { get => _select; set => _select = value; }

        private bool _resetName = false;
        public bool SkipName { get => _resetName; set => _resetName = value; }

        private IFilter? _filter;
        public IFilter? Filter { get => _filter; set => _filter = value; }

        private List<Override> _overrides = new();
        public List<Override> Overrides { get => _overrides; set => _overrides = value; }

        private float _weighting = 1.0F;
        public float Weighting { get => _weighting; set => _weighting = value; }

        private List<Variant> _variants = new();
        public List<Variant> Variants { get => _variants; set => _variants = value; }

        public int ForceCount => (_filter == null) ? 0 : FilterForceCount(_filter);
        private int FilterForceCount(IFilter filter)
        {
            int count = (filter.Extensions.ContainsKey("Force") && ((bool)filter.Extensions["Force"]! == true)) ? 1 : 0;
            if (filter is GroupFilter) count += ((GroupFilter)filter).Filters.Sum(f => FilterForceCount(f));
            return count;
        }

        public override string ToString()
        {
            string output = (_name == null) ? string.Empty : $"{_name} - ";
            if (_overrides.Count > 0) output += $"O{_overrides.Count} ";
            if (_variants.Count > 0) output += $"V{_variants.Count} ";
            output += $"W{_weighting} ";
            output += $"FC{ForceCount}";
            return output;
        }

        public List<Variant> Flatten()
        {
            List<Variant> output = new();

            output.Add(this);

            output.AddRange(_variants
                .Where(v => !v.Type.HasFlag(VariantType.Addon))
                .SelectMany(v => v.Flatten())
                .Select(v => Merge(v))
                .Where(v => v != null)
                .Select(v => v!));

            List<List<Variant>> addonGroups = _variants
                .Where(v => v.Type.HasFlag(VariantType.Addon))
                .Select(v => v.Flatten())
                .ToList();

            foreach (List<Variant> addonGroup in addonGroups)
            {
                foreach (Variant variant in output.ToList())
                {
                    foreach (Variant addon in addonGroup)
                    {
                        Variant? merged = variant.AddAddon(addon);
                        if (merged != null) output.Add(merged);
                    }
                }
            }

            if (_type.HasFlag(VariantType.Group) || ((_overrides.Count == 0) && (_variants.Count > 0))) output.Remove(this);

            return output;
        }

        public Variant? Merge(Variant other)
        {
            Variant output = new();
            output.Name = (string.IsNullOrEmpty(_name) || SkipName) ? other.Name : $"{_name}.{other.Name}";
            output.Type = _type & ~VariantType.Group;
            output.Select = VariantSelectMode.Single;
            output.Filter = FilterUtils.Merge(_filter, other.Filter);
            List<Override>? mergedOverrides = OverrideUtils.Merge(_overrides, other.Overrides);
            if (mergedOverrides == null) return null;
            output.Overrides = mergedOverrides;
            output.Weighting = _weighting * other.Weighting;
            return output;
        }

        public Variant? AddAddon(Variant addon)
        {
            Variant? output = Merge(addon);
            if (output != null) output.Name = $"{_name} +{addon.Name}";
            return output;
        }
    }
}
