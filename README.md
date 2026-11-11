# Stand Area

Combine unevenly sized inventory plots while retaining individual stem measurements.

## Try it

From this folder with the .NET 10 SDK installed:

```sh
dotnet run -- inventory study.tsv
```

Use plot, plot ID, area in m². Use stem, plot ID, unique stem ID within that plot, species, diameter in cm. Empty plots count toward total sampled area.

The included study.tsv uses tab-separated fields. Comment lines start with `#`.

The original exercise remains available with `dotnet run -- example.tsv`.

## When to use it

A forestry student summarizes an inventory using two fixed-area plots of different sizes.

Example result:

```text
N: 1 stems, 3.142 m²/ha
  Oak: 3.142 m²/ha
S: 1 stems, 4.189 m²/ha
  Oak: 4.189 m²/ha
pooled Oak: 3.927 m²/ha
pooled all: 3.927 m²/ha
```

Related course or field workflow: [US Forest Service inventory techniques](https://research.fs.usda.gov/sites/default/files/2023-02/california_inventory_techniques_manual.pdf).
