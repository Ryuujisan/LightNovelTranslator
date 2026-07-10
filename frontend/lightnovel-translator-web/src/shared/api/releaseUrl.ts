
type GitHubRelease = {
    tag_name: string;
    html_url: string;
    published_at: string | null;
};

const releasesUrl = "https://api.github.com/repos/Ryuujisan/LightNovelTranslator/releases";

function getVersionParts(tagName: string) {
    const version = tagName.match(/\d+/g);

    return version?.map(Number) ?? [];
}

function compareApiTags(a: GitHubRelease, b: GitHubRelease) {
    const aVersion = getVersionParts(a.tag_name);
    const bVersion = getVersionParts(b.tag_name);
    const length = Math.max(aVersion.length, bVersion.length);

    for (let i = 0; i < length; i += 1) {
        const diff = (bVersion[i] ?? 0) - (aVersion[i] ?? 0);

        if (diff !== 0) {
            return diff;
        }
    }

    return (
        new Date(b.published_at ?? 0).getTime() -
        new Date(a.published_at ?? 0).getTime()
    );
}

export async function getLatestApiReleaseUrl() {
    const response = await fetch(
        releasesUrl
    );

    if (!response.ok) {
        throw new Error(`GitHub API error: ${response.status}`);
    }

    const releases = (await response.json()) as GitHubRelease[];

    const latestApi = releases
        .filter((release) => release.tag_name.toLowerCase().startsWith("api"))
        .sort(compareApiTags)[0];

    if (!latestApi) {
        throw new Error("No API release found");
    }

    return latestApi.html_url;
}
