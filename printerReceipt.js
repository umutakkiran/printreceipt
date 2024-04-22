const fs = require("fs");

let line = 1;
const lines = [];

fs.readFile("response.json", "utf8", (err, data) => {
    if (err) {
        console.error("Dosya okunurken bir hata oluştu:", err);
        return;
    }

    try {
        const result = JSON.parse(data);
        result.shift();
        const corDictionary = result
            .map(({ description, boundingPoly }) => {
                return {
                    points: minMaxYKoordinatiAl(boundingPoly["vertices"]),
                    description: description,
                    status: false,
                };
            })
            .sort((a, b) => {
                return a.points[0] - b.points[0];
            });

        for (var i = 0; i < corDictionary.length; i++) {
            const element = corDictionary[i];

            lines.push({
                line: line,
                sentences: [element.description],
                points: element.points,
            });
            const el = corDictionary.find(
                (desc) => desc.description === element.description
            );
            el.status = true;
            for (var k = i + 1; k < corDictionary.length; k++) {
                const val = corDictionary[k];

                if (
                    val.points[0] >= element.points[0] &&
                    val.points[0] < element.points[1]
                ) {
                    i++;
                    const lineRes = lines[lines.findIndex((el) => el.line === line)];
                    lineRes.sentences = [...lineRes.sentences, val.description];
                    const el = corDictionary.find(
                        (desc) => desc.description === val.description
                    );
                    el.status = true;
                } else {
                    if (k == corDictionary.length - 1) {
                        line++;
                        break;
                    }
                }
            }
        }
        lines.map((line) =>
            console.log("line", line.line, line.sentences.join(" "))
        );
    } catch (error) {
        console.log(error);
    }
});

function minMaxYKoordinatiAl(array) {
    let minY = Infinity;
    let maxY = -Infinity;

    array.forEach((point) => {
        minY = Math.min(minY, point.y);
        maxY = Math.max(maxY, point.y);
    });

    return [minY, maxY];
}