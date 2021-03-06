﻿using Moq;
using System;
using System.Linq;
using System.Runtime.Serialization;
using TerrainQuest.Generator;
using TerrainQuest.Generator.Effects;
using TerrainQuest.Generator.Graph;
using TerrainQuest.Generator.Helpers;
using Xunit;

namespace TerrainQuest.Tests.Generator.Graph
{
    public class ImageEffectNodeTests
    {
        [Fact]
        public void Constructor_InitializesSourceAndImageEffect()
        {
            // Arrange
            var source = new Mock<HeightMapNode>().Object;
            var effect = new Mock<IEffect>().Object;

            // Act
            var actual = new ImageEffectNode(source, effect);

            // Assert
            Assert.Equal(source, actual.Source);
            Assert.Equal(effect, actual.Effects.Single());
        }

        [Fact]
        public void Constructor_InitializesSourceAndMultipleEffects()
        {
            // Arrange
            var source = new Mock<HeightMapNode>().Object;
            var effectA = new Mock<IEffect>().Object;
            var effectB = new Mock<IEffect>().Object;

            // Act
            var actual = new ImageEffectNode(source, effectA, effectB);

            // Assert
            Assert.Equal(source, actual.Source);
            Assert.Equal(effectA, actual.Effects.First());
            Assert.Equal(effectB, actual.Effects.Skip(1).First());
        }

        [Fact]
        public void Constructor_InitializesSourceWithNoEffects()
        {
            // Arrange
            var source = new Mock<HeightMapNode>().Object;

            // Act
            var actual = new ImageEffectNode(source);

            // Assert
            Assert.Equal(source, actual.Source);
            Assert.NotNull(actual.Effects);
            Assert.Empty(actual.Effects);
        }

        [Fact]
        public void Constructor_NullSourceThrowsArgumentNullException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => {
                // Act
                var actual = new ImageEffectNode(null);
            });
        }

        [Fact]
        public void Dependencies_ReturnsSource()
        {
            // Arrange
            var source = new Mock<HeightMapNode>().Object;

            // Act
            var actual = new ImageEffectNode(source);

            // Assert
            Assert.Equal(source, actual.Dependencies.Single());
        }

        [Fact]
        public void AddEffect_AppendsEffectToList()
        {
            // Arrange
            var source = new Mock<HeightMapNode>().Object;
            var expected = new Mock<IEffect>().Object;
            var node = new ImageEffectNode(source);

            // Act
            node.AddEffect(expected);

            // Assert
            Assert.Equal(expected, node.Effects.Single());
        }

        [Fact]
        public void AddEffect_NullEffectThrowsArgumentNullException()
        {
            // Arrange
            var source = new Mock<HeightMapNode>().Object;
            var node = new ImageEffectNode(source);

            // Assert
            Assert.Throws<ArgumentNullException>(() => {
                // Act
                node.AddEffect(null);
            });
        }

        [Fact]
        public void ISerializable_SerializeAndDeserializeCorrectly()
        {
            // Arrange
            var source = new Mock<HeightMapNode>().Object;
            var effectA = new Mock<IEffect>().Object;
            var effectB = new Mock<IEffect>().Object;
            var expected = new ImageEffectNode(source, effectA, effectB);
            var info = MockSerializationInfo<ImageEffectNode>();

            // Act
            expected.GetObjectData(info, new StreamingContext());
            var actual = new ImageEffectNode(info, new StreamingContext());

            // Assert
            Assert.Equal(expected.Source, actual.Source);
            Assert.Equal(expected.Effects.First(), actual.Effects.First());
            Assert.Equal(expected.Effects.Skip(1).First(), actual.Effects.Skip(1).First());
        }

        [Fact]
        public void Execute_RunsCalculateOnAllEffects()
        {
            // Arrange
            var sourceMock = new Mock<HeightMapNode>();
            var mockEffectA = new Mock<IEffect>();
            mockEffectA.Setup(m => m.Calculate(It.IsAny<HeightMap>()));
            var mockEffectB = new Mock<IEffect>();
            mockEffectB.Setup(m => m.Calculate(It.IsAny<HeightMap>()));
            var node = new ImageEffectNode(sourceMock.Object, mockEffectA.Object, mockEffectB.Object);

            // Act
            node.Execute();

            // Assert
            mockEffectA.Verify(m => m.Calculate(It.IsAny<HeightMap>()), Times.Once);
            mockEffectB.Verify(m => m.Calculate(It.IsAny<HeightMap>()), Times.Once);
        }

        // Test that the effects are actually applied to heightmap
        [Fact]
        public void Execute_AllEffectsAreAppliedToHeightMap()
        {
            // Arrange
            var data = new double[,] { { 0d, 0.25d, 0.5d, 0.75d, 1d } };
            var sourceMock = new Mock<HeightMapNode>();
            sourceMock.SetupGet(h => h.Result)
                .Returns(new HeightMap(data));
            var expected = MockImageEffect(MockImageEffect(MockImageEffect(
                sourceMock.Object.Result))).Data;

            var mockEffectA = new Mock<IEffect>();
            mockEffectA.Setup(m => m.Calculate(It.IsAny<HeightMap>()))
                .Returns<HeightMap>(MockImageEffect);
            var node = new ImageEffectNode(sourceMock.Object,
                mockEffectA.Object, mockEffectA.Object, mockEffectA.Object);

            // Act
            node.Execute();
            var actual = node.Result.Data;

            // Assert
            Assert.Equal(expected[0, 0], actual[0, 0]);
            Assert.Equal(expected[0, 1], actual[0, 1]);
            Assert.Equal(expected[0, 2], actual[0, 2]);
            Assert.Equal(expected[0, 3], actual[0, 3]);
            Assert.Equal(expected[0, 4], actual[0, 4]);
        }

        [Fact]
        public void Execute_ReturnsSourceWhenNoEffectsAreAdded()
        {
            // Arrange
            var expected = new double[,] { { 0d, 0.25d, 0.5d, 0.75d, 1d } };
            var sourceMock = new Mock<HeightMapNode>();
            sourceMock.SetupGet(h => h.Result)
                .Returns(new HeightMap(expected));
           var node = new ImageEffectNode(sourceMock.Object);

            // Act
            node.Execute();
            var actual = node.Result.Data;

            // Assert
            Assert.Equal(expected[0, 0], actual[0, 0]);
            Assert.Equal(expected[0, 1], actual[0, 1]);
            Assert.Equal(expected[0, 2], actual[0, 2]);
            Assert.Equal(expected[0, 3], actual[0, 3]);
            Assert.Equal(expected[0, 4], actual[0, 4]);

        }

        private HeightMap MockImageEffect(HeightMap h)
        {
            var result = h.Clone();

            h.ForEach((r, c) => {
                result[r, c] += 0.1d; 
            });

            return result;
        }

        private static SerializationInfo MockSerializationInfo<T>()
        {
            var formatterConverter = new Mock<IFormatterConverter>().Object;
            return new SerializationInfo(typeof(T), formatterConverter);
        }
    }
}
